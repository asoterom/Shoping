using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoping.Common;
using Shoping.Data;
using Shoping.Data.Entities;
using Shoping.Enums;
using Shoping.Helpers;
using Shoping.Models;

namespace Shoping.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UsersController :Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IMailHelper _mailHelper;

        public UsersController(DataContext context, IUserHelper userHelper ,IBlobHelper blobHelper,
            ICombosHelper combosHelper,IMailHelper mailHelper  )
        {
            _context = context;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
            _combosHelper = combosHelper;
            _mailHelper = mailHelper;
        }

        public async Task<IActionResult> Index()
        {
            //la tabla user no esta en las entidades declaradas del datatcontext
            //esta viende del identitydatacontext
            return View(await _context.Users
                .Include(u => u.City)  //relacion de primer nivel segun diagrama entidad relacion
                .ThenInclude(c => c.State)   //relacion de segundo nivel segun diagrama entidad relacon
                .ThenInclude(s => s.Country)   //relacion de tercer nivel segun diagrama entidad relacon
                .ToListAsync());
        }


        public async Task<IActionResult> Create()
        {
            AddUserViewModel model = new()
            {
                Id = Guid.Empty.ToString(),
                Countries = await _combosHelper.GetComboCountriesAsync(),
                States = await _combosHelper.GetComboStatesAsync(0),
                Cities = await _combosHelper.GetComboCitiesAsync(0),
                UserType = UserType.Admin ,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;

                if (model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                }

                model.ImageId = imageId;
                User user = await _userHelper.AddUserAsync(model);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Este correo ya está siendo usado.");
                    model.Countries = await _combosHelper.GetComboCountriesAsync();
                    model.States = await _combosHelper.GetComboStatesAsync(model.CountryId);
                    model.Cities = await _combosHelper.GetComboCitiesAsync(model.StateId);
                    return View(model);
                }

                //registrado el usuario se genera el token para confirmar correo
                string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendMail(
                    $"{model.FirstName} {model.LastName}",
                    model.Username,
                    "Shopping - Confirmación de Email",
                    $"<h1>Shopping - Confirmación de Email</h1>" +
                        $"Para habilitar el usuario por favor hacer click en el siguiente link:, " +
                        $"<br><br><p><a href = \"{tokenLink}\">Confirmar Email</a></p>");
                if (response.IsSuccess)
                {
                    ViewBag.Message = "Las instrucciones para habilitar el administrador han sido enviadas al correo.";
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, response.Message);


               // return RedirectToAction("Index", "Home");
            }

            model.Countries = await _combosHelper.GetComboCountriesAsync();
            model.States = await _combosHelper.GetComboStatesAsync(model.CountryId);
            model.Cities = await _combosHelper.GetComboCitiesAsync(model.StateId);
            return View(model);
        }


        public JsonResult GetStates(int countryId)
        {
            Country country = _context.Countries
                .Include(c => c.States)
                .FirstOrDefault(c => c.Id == countryId);
            if (country == null)
            {
                return null;
            }

            return Json(country.States.OrderBy(d => d.Name));
        }

        public JsonResult GetCities(int stateId)
        {
            State state = _context.States
                .Include(s => s.Cities)
                .FirstOrDefault(s => s.Id == stateId);
            if (state == null)
            {
                return null;
            }

            return Json(state.Cities.OrderBy(c => c.Name));
        }



    }
}
