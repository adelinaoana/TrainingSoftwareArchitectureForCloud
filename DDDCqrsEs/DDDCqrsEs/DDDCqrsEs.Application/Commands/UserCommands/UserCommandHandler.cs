using DDDCqrsEs.Application.Models.AuthenticationModels;
using MediatR;
using DDDCqrsEs.Common.Localization;
using System.Threading;
using System.Threading.Tasks;

namespace DDDCqrsEs.Application.Commands.UserCommands
{
    public class UserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserCommandReponse>,
      IRequestHandler<DeleteUserCommand, Unit>,
      IRequestHandler<ChangePasswordCommand, ChangePasswordCommandReponse>,
      IRequestHandler<LoginCommand, LoginCommandResponse>,
      IRequestHandler<LogoutCommand, Unit>
    {
        private readonly IUserService _userService;
        private readonly Ii18nService _ii18NService;
        public UserCommandHandler(IUserService userService, Ii18nService ii18NService)
        {
            _userService = userService;
            _ii18NService = ii18NService;
        }

        public async Task<RegisterUserCommandReponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            return new RegisterUserCommandReponse { User = _userService.Register(request.User) };
        }

        public Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            _userService.DeleteUser(request.UserId);
            _userService.SignOut();
            return Unit.Task;
        }

        public async Task<LoginCommandResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = _userService.SignIn(request.User.Username, request.User.Password);
            return new LoginCommandResponse { User = user };
        }

        public Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            _userService.SignOut();
            return Unit.Task;
        }

        public async Task<ChangePasswordCommandReponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var email = request.Username;
            var isSuccessfull = _userService.ChangePassword(email, request.OldPassword, request.NewPassword);

            return new ChangePasswordCommandReponse { IsSucessful = isSuccessfull };
        }
    }
}
