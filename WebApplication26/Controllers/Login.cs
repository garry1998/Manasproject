using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using WebApplication26.Models;

namespace WebApplication26.Controllers
{
    public class Login : Controller
    {


        private readonly project1211Context _context;
        public Login(project1211Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            return PartialView();
        }
        public IActionResult Logout()
        {
            //HttpContext.Session.SetString("name","");
            //HttpContext.Session.SetString("Type","");
            //HttpContext.Session.SetString("Email","");
            HttpContext.Session.Clear();
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");




        }
        [HttpPost]
        public ActionResult Index(Logincs objs)

        { var ac= (from s in _context.MstUsers
                   where s.Email == objs.Username 
                   select s).SingleOrDefault();

            ClaimsIdentity identity = null;
            bool isAuthenticated = false;

            if ((!ModelState.IsValid) || ac == null)
            {
                ModelState.AddModelError("", "Failed");
                return PartialView(objs);
            }
            else
            {
                if (ac.IsActive == false) 
                {
                    ModelState.AddModelError("", "Account not approved");
                    return PartialView(objs);
                }
                else if ((objs.Username == ac.Email) && (objs.Password == ac.Pswd) && (objs.checktype == ac.FkRoleId))

                {
                    if (objs.checktype == 1)
                    {

                       
                        identity = new ClaimsIdentity(new[] {
                      new Claim(ClaimTypes.Name,ac.Fname),
                      new Claim(ClaimTypes.Role, "Student")
                      }, CookieAuthenticationDefaults.AuthenticationScheme);

                        
                            var principal = new ClaimsPrincipal(identity);

                            var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                            HttpContext.Session.SetString("Email", ac.Email);
                            HttpContext.Session.SetString("name", ac.Fname + ac.Lname);
                            HttpContext.Session.SetString("Type", ac.FkRoleId.ToString());

                            return RedirectToAction("Myprofilee", "StudentDetails");
                        
                       
                       
                       
                    }
                    else
                    {
                       
                        //this.Session.SetString("TransId", "x001");
                        //Session["UserId"] = Guid.NewGuid();
                        if (objs.checktype == 2)
                        {
                            identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, ac.Fname),
                          new Claim(ClaimTypes.Role, "Faculty")
                         }, CookieAuthenticationDefaults.AuthenticationScheme);
                        }
                        if (objs.checktype == 3)
                        {
                            identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, ac.Fname),
                    new Claim(ClaimTypes.Role, "Admin")
                }, CookieAuthenticationDefaults.AuthenticationScheme);
                        }

                   
                        var principal = new ClaimsPrincipal(identity);

                        var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                            HttpContext.Session.SetString("Email", ac.Email);
                            HttpContext.Session.SetString("name", ac.Fname + ac.Lname);
                            HttpContext.Session.SetString("Type", ac.FkRoleId.ToString());
                            return RedirectToAction("Index", "Home");
                    
                       
                    }
                }

                else
                {
                    ModelState.AddModelError("", "Failed");
                    return PartialView(objs);
                }


            }
        }
    }
}
