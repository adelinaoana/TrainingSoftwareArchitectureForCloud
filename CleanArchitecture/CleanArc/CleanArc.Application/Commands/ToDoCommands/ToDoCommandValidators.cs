using FluentValidation;
using CleanArc.Application.Interfaces;
using CleanArc.Application.Models;
using CleanArc.Common.Errors;
using CleanArc.Common.Localization;
using CleanArc.Domain.Entities;
using System;
using System.Linq;
using System.Security.AccessControl;
using CleanArc.Application.Common;

namespace CleanArc.Application.Commands.ToDoCommands
{
    public class AddToDoCommandValidator : AbstractValidator<AddToDoCommand>
    {
        private readonly IRepository repository;

        public AddToDoCommandValidator(IRepository repository,
            Ii18nService i18nService)
        {
            this.repository = repository;
            RuleFor(x => x.ToDo).Must(BeUnique).WithMessage(string.Format(i18nService.GetMessage(ErrorMessagesNames.DUPLICATE_ENTITY), nameof(ToDo)));
            RuleFor(x => x.ToDo.Name).Must(NotNullOrEmpty).WithMessage(string.Format(i18nService.GetMessage(ErrorMessagesNames.REQUIRED), nameof(ToDo.Name)));
            RuleFor(x => x.ToDo).Must(NotAlreadyExistsImportant).WithMessage(string.Format(i18nService.GetMessage(ErrorMessagesNames.DUPLICATE_IMPORTANT_ENTITY), nameof(ToDo.Name)));
        }

        private bool NotAlreadyExistsImportant(ToDoModel toDoModel)
        {
            if (toDoModel == null || !toDoModel.Name.Contains(Constants.IMPORTANT))
                return true;

            var toDo = repository.GetEntities<ToDo>().FirstOrDefault(x => x.Name.Equals(Constants.IMPORTANT));

            return toDo == null;
        }

        private bool BeUnique(ToDoModel toDoModel)
        {
            var toDo = repository.GetEntities<ToDo>().FirstOrDefault(x=> x.Name == toDoModel.Name);
            if (toDo != null && toDo.Id != toDoModel.Id)
            {
                return false;
            }
            return true;
        }

        private bool NotNullOrEmpty(string text)
        {
            return !string.IsNullOrEmpty(text);
        }
    }


    public class EditToDoCommandValidator : AbstractValidator<EditToDoCommand>
    {
        private readonly IRepository repository;

        public EditToDoCommandValidator(IRepository repository,
            Ii18nService i18nService)
        {
            this.repository = repository;
            
            RuleFor(x => x.ToDo).Must(BeUnique).WithMessage(string.Format(i18nService.GetMessage(ErrorMessagesNames.DUPLICATE_ENTITY), nameof(ToDo)));
            RuleFor(x => x.ToDo.Name).Must(NotNullOrEmpty).WithMessage(string.Format(i18nService.GetMessage(ErrorMessagesNames.REQUIRED), nameof(ToDo.Name)));
            RuleFor(x => x.ToDo.Id).Must(Exist).WithMessage(string.Format(i18nService.GetMessage(ErrorMessagesNames.NOT_FOUND), nameof(ToDo)));
        }

        private bool BeUnique(ToDoModel toDoModel)
        {
            var toDo = repository.GetEntities<ToDo>().FirstOrDefault(x => x.Name == toDoModel.Name);
            if (toDo != null && toDo.Id != toDoModel.Id)
            {
                return false;
            }
            return true;
        }

        private bool Exist(Guid? id)
        {
            var toDo = repository.GetEntities<ToDo>().FirstOrDefault(x => x.Id == id);
            if (toDo == null)
            {
                return false;
            }
            return true;
        }
        private bool NotNullOrEmpty(string text)
        {
            return !string.IsNullOrEmpty(text);
        }
    }
}




