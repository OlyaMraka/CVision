using AutoMapper;
using CVision.BLL.DTOs.Users;
using CVision.BLL.Interfaces;
using CVision.DAL.Entities;
using FluentResults;
using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace CVision.BLL.Commands.Users.Register;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<RegisterUserResponseDto>>
{
    private readonly IEmailService _emailService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IValidator<RegisterUserCommand> _registerUserRequestDtoValidator;

    public RegisterUserHandler(
        IEmailService emailService,
        UserManager<ApplicationUser> userManager,
        IMapper mapperObj,
        IValidator<RegisterUserCommand> registerUserRequestDtoValidator)
    {
        _emailService = emailService;
        _userManager = userManager;
        _mapper = mapperObj;
        _registerUserRequestDtoValidator = registerUserRequestDtoValidator;
    }

    public async Task<Result<RegisterUserResponseDto>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _registerUserRequestDtoValidator
            .ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Fail<RegisterUserResponseDto>(validationResult.Errors.First().ErrorMessage);
        }

        ApplicationUser newUser = _mapper.Map<ApplicationUser>(request.RequestDto);

        var identityResult = await _userManager.CreateAsync(newUser, request.RequestDto.Password);

        if (!identityResult.Succeeded)
        {
            return Result.Fail<RegisterUserResponseDto>(identityResult.Errors.Select(e => e.Description));
        }

        var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

        var confirmationLink = $"https://translate.google.com/?hl=uk&sl=uk&tl=en&op=translate";

        await _emailService.SendConfirmationEmailAsync(newUser.Email!, confirmationLink);

        var responseDto = _mapper.Map<RegisterUserResponseDto>(newUser);
        return Result.Ok(responseDto);
    }
}