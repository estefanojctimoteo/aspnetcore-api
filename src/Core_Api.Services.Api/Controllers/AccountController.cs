using System;
using System.Linq;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

using MediatR;
using AutoMapper;

using Core_Api.Domain.Interfaces;
using Core_Api.Domain.Core.Notifications;
using Core_Api.Domain.AppUsers.Commands;
using Core_Api.Domain.AppUsers.Repository;

using Core_Api.Infra.CrossCutting.Identity.Models;
using Core_Api.Infra.CrossCutting.Identity.Models.AccountViewModels;
using Core_Api.Infra.CrossCutting.Identity.Authorization;

using Core_Api.Services.Api.ViewModels;

namespace Core_Api.Services.Api.Controllers
{
    public class AccountController : BaseController
    {
        #region Interfaces

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IMediatorHandler _mediator;
        private readonly TokenDescriptor _tokenDescriptor;
        //private readonly Bugsnag.IClient _bugsnag;

        #endregion

        #region Constructor

        public AccountController(
                    UserManager<ApplicationUser> userManager,
                    SignInManager<ApplicationUser> signInManager,
                    ILoggerFactory loggerFactory,
                    IMapper mapper,
                    TokenDescriptor tokenDescriptor,
                    INotificationHandler<DomainNotification> notifications, 
                    IUser aspNetUser,
                    IAppUserRepository appUserRepository,
                    IMediatorHandler mediator
                    // , Bugsnag.IClient bugsnag
            ) : base(notifications, aspNetUser, mediator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appUserRepository = appUserRepository;
            _mediator = mediator;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _mapper = mapper;
            _tokenDescriptor = tokenDescriptor;
            //_bugsnag = bugsnag;
        }

        #endregion

        #region ToUnixEpochDate

        private static long ToUnixEpochDate(DateTime date)
      => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        #endregion


        #region AppUsers Methods

        #region GetAll

        [HttpGet]
        [Authorize(Policy = "CanReadUsers")]
        [Route("usuarios")]
        public IEnumerable<AppUserViewModel> Get()
        {
            return _mapper.Map<IEnumerable<AppUserViewModel>>(_appUserRepository.GetAll());
        }

        #endregion

        #region ObterListaAspNetUserClaims, ObterListaTiposClaims e UpdateAspNetUserClaims

        #region ObterListaAspNetUserClaims

        [HttpGet]
        [Authorize(Policy = "CanReadUsers")]
        [Route("appusers/aspnetuserclaims/{id:guid}")]
        public async Task<object> ObterListaAspNetUserClaims(Guid id)
        {
            var userEF = await _userManager.FindByIdAsync(id.ToString());
            var userClaims = await _userManager.GetClaimsAsync(userEF);
            return userClaims;
        }

        #endregion

        #region ObterListaTiposClaims

        [HttpGet]
        [Authorize(Policy = "CanReadUsers")]
        [Route("claims")]
        public IEnumerable<ClaimViewModel> GetAllClaimsType()
        {
            return _mapper.Map<IEnumerable<ClaimViewModel>>(_appUserRepository.GetAllClaimsType());
        }

        #endregion

        #region UpdateAspNetUserClaims

        [HttpPost]
        [Authorize(Policy = "CanUpdateUsers")]
        [Route("appusers/updateaspnetuserclaims")]
        public async Task<object> UpdateAspNetUserClaims([FromBody] UserClaimsViewModel model)
        {
            try
            {
                var userEF = await _userManager.FindByIdAsync(model.aspnetuserid);
                var oldUserClaims = await _userManager.GetClaimsAsync(userEF);
                foreach (Claim oldClaim in oldUserClaims)
                {
                    await _userManager.RemoveClaimAsync(userEF, oldClaim);
                }
                List<Claim> newUserClaims = new List<Claim>();
                foreach(ClaimViewModel _claim in model.claims)
                {
                    newUserClaims.Add(new Claim(_claim.type, _claim.value));
                }
                await _userManager.AddClaimsAsync(userEF, newUserClaims);
                var _newUserClaims = await _userManager.GetClaimsAsync(userEF);
                return _newUserClaims;
            }
            catch (Exception e)
            {
                //_bugsnag.Notify(new Exception(e.Message));
                return null;
            }
        }

        #endregion

        #endregion

        #region Novo usuário / nova-conta

        [HttpPost]
        //[AllowAnonymous]
        [Route("new-account")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model, int version)
        {
            if (version == 2)
            {
                return Response(new { Message = "API V2 não disponível" });
            }

            var userEF = await _userManager.FindByEmailAsync(model.Email);
            if (userEF != null && !string.IsNullOrEmpty(userEF.Email))
            {
                return Response_BadRequest("Já existe usuário cadastrado com o email informado.");                
            }

            if (model.FuncionarioId <= 0 && model.ProfissionalId <= 0)
            {
                return Response_BadRequest("O ID do funcionário ou o ID do profisssional deve ser informado.");
            }

            if (model.FuncionarioId > 0 && model.ProfissionalId > 0)
            {
                return Response_BadRequest("O ID do funcionário e o ID do profisssional não podem ser informados ao mesmo tempo.");                
            }

            if (!ModelState.IsValid)
            {
                NotifyErrorInvalidModel();
                return Response();
            }

            userEF = new ApplicationUser { UserName = model.Email, Email = model.Email };

            var result = await _userManager.CreateAsync(userEF, model.Senha);

            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(userEF, new Claim("Tipos", "Ler"));
                try
                {
                    var registerCommand = new RegisterAppUserCommand(Guid.Parse(userEF.Id), userEF.Email);
                    await _mediator.SendCommand(registerCommand);

                    if (!InvalidOperation())
                    {
                        await _userManager.DeleteAsync(userEF);
                        return Response(model);
                    }

                    var usuarioCore_Api = _appUserRepository.GetById(Guid.Parse(userEF.Id));

                    if (usuarioCore_Api == null || usuarioCore_Api.UserID != Guid.Parse(userEF.Id))
                    {
                        await _userManager.DeleteAsync(userEF);
                        throw new Exception("Couldn't create the user.");
                    }

                    _logger.LogInformation(1, "Sucess!");
                    var response = new { Message = "Sucess!", NovoUserID = userEF.Id };
                    return Response(response);
                }
                catch (Exception exc1)
                {
                    _appUserRepository.Remove(Guid.Parse(userEF.Id));
                    await _userManager.DeleteAsync(userEF);
                    var response = new { Message = "Couldn't create the user: " + exc1.Message };
                    //_bugsnag.Notify(new Exception(exc1.Message));
                    return Response(response);
                }
            }
            AddIdentityErrors(result);
            return Response(model);
        }

        #endregion

        #region ObterUsuarioPorId

        [HttpGet]
        [Authorize(Policy = "CanReadUsers")]
        [Route("appusers/{id:guid}")]
        public IActionResult GetUserById(Guid id)
        {
            var appuser = _mapper.Map<AppUserViewModel>(_appUserRepository.GetById(id));
            return appuser == null ? StatusCode(404) : Response(appuser);
        }

        #endregion

        #region Alteração de senha

        [HttpPost]        
        [Route("alterarminhasenha")]
        public async Task<IActionResult> AlterarSenhaUsuarioLogado([FromBody] UpdatePasswordViewModel model, int version)
        {
            if (version == 2)
            {
                return Response(new { Message = "API V2 não disponível" });
            }

            var userEF = await _userManager.FindByIdAsync(model.Id);
            if (userEF == null || string.IsNullOrEmpty(userEF.Email) || userEF.Email != model.Email)
            {
                return Response(new { Message = "Não foi possível localizar o usuário na base de dados." });
            }

            if (!ModelState.IsValid)
            {
                NotifyErrorInvalidModel();
                return Response();
            }

            var result = await _userManager.ChangePasswordAsync(userEF, model.SenhaAtual, model.NovaSenha);

            if (result.Succeeded)
            {
                _logger.LogInformation(1, "Senha alterada com sucesso!");
                var response = new { Message = "Senha alterada com sucesso!" };
                return Response(response);
            }
            AddIdentityErrors(result);
            return Response(model);
        }

        #endregion

        #region ExcluirUsuario

        [HttpDelete]
        [Route("appusers/{id:guid}")]
        [Authorize(Policy = "CanUpdateUsers")]
        public IActionResult ExcluirUsuario(Guid id)
        {
            var usuarioViewModel = new AppUserViewModel { UserID = id };
            var usuarioCommand = _mapper.Map<RemoveAppUserCommand>(usuarioViewModel);

            _mediator.SendCommand(usuarioCommand);
            return Response(usuarioCommand);
        }

        #endregion

        #region Login

        [HttpPost]
        [AllowAnonymous]
        [Route("conta")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyErrorInvalidModel();
                return Response(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, true);

            if (result.Succeeded)
            {
                var userEF = await _userManager.FindByEmailAsync(model.Email);
                var usuarioCore = _appUserRepository.GetById(Guid.Parse(userEF.Id));

                string userName = string.Empty;

                _logger.LogInformation(1, "Login succed");
                var response = GenerateAppUserToken(model, usuarioCore, userName);
                return Response(response);
            }

            NotifyError(result.ToString(), "Falha ao realizar o login");
            return Response(model);
        }

        #endregion

        #region GenerateAppUserToken

        private async Task<object> GenerateAppUserToken(LoginViewModel login, Domain.AppUsers.AppUser usuarioCore, string userName)
        {
            var aspnetUser = await _userManager.FindByEmailAsync(login.Email);
            var userClaims = await _userManager.GetClaimsAsync(aspnetUser);

            userClaims.Add(new Claim(JwtRegisteredClaimNames.Sub, aspnetUser.Id));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Email, aspnetUser.Email));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
            userClaims.Add(new Claim("name", (!string.IsNullOrEmpty(userName.Trim()) ? userName : usuarioCore.Email)));

            // Necessário converver para IdentityClaims
            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(userClaims);

            var handler = new JwtSecurityTokenHandler();
            var signingConf = new SigningCredentialsConfiguration();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _tokenDescriptor.Issuer,
                Audience = _tokenDescriptor.Audience,
                SigningCredentials = signingConf.SigningCredentials,
                Subject = identityClaims,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(_tokenDescriptor.MinutesValid),                
            });

            var encodedJwt = handler.WriteToken(securityToken);

            var response = new
            {
                access_token = encodedJwt,
                expires_in = DateTime.Now.AddMinutes(_tokenDescriptor.MinutesValid),
                usuario = new
                {
                    id = usuarioCore.UserID,
                    claims = userClaims.Select(c => new { c.Type, c.Value }),
                    name = !string.IsNullOrEmpty(userName.Trim()) ? userName : usuarioCore.Email,
                    email = usuarioCore.Email
                }
            };

            return response;
        }

        #endregion

        #endregion
    }
}