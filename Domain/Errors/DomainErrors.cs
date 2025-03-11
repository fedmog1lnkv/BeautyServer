using Domain.Shared;

namespace Domain.Errors;

public class DomainErrors
{
    public static class Season
    {
        public static Error UserAlreadyRegistered(Guid userId) => Error.Conflict(
            "User.UserAlreadyRegistered", $"User with the Id '{userId}' was already registered");

        public static readonly Error NameNotUnique = Error.Conflict(
            "SeasonName.NameNotUnique", "Season name must be unique.");
        
        public static Error GameAlreadyExist(Guid gameId) => Error.Conflict(
            "Game.GameAlreadyExist", $"Game with the Id '{gameId}' already exists");
    }

    public static class Game
    {
        public static Error NotFound(Guid id) => Error.NotFound(
            "Game.NotFound", $"Game with the Id '{id}' was not found.");

        public static Error NotFoundByUnit(Guid unitId) => Error.NotFound(
            "Game.NotFound", $"Game with the unit Id '{unitId}' was not found.");

    }

    public static class User
    {
        public static Error NotFound(Guid id) => Error.NotFound(
            "User.NotFound", $"User with the Id '{id}' was not found.");

        public static Error NotFoundByUnit(Guid unitId) => Error.NotFound(
            "User.NotFound", $"User with the unit Id '{unitId}' was not found.");

        public static readonly Error NameNotUnique = Error.Conflict(
            "UserName.NameNotUnique", "User name must be unique.");
    }
    
    public static class Role
    {
        public static Error NotFound(Guid id) => Error.NotFound(
            "Role.NotFound", $"Role with the Id '{id}' was not found.");

        public static Error NotFoundByUnit(Guid unitId) => Error.NotFound(
            "Role.NotFound", $"Role with the unit Id '{unitId}' was not found.");

        public static readonly Error NameNotUnique = Error.Conflict(
            "RoleName.NameNotUnique", "Role name must be unique.");
    }
    public static class UserName
    {
        public static readonly Error IsTheSame = Error.Validation(
            "UserName.IsTheSame", "It is impossible to change the name to the same.");

        public static readonly Error Empty = Error.Validation(
            "UserName.Empty", "User name is empty.");

        public static readonly Error TooLong = Error.Validation(
            "UserName.TooLong", "User name is too long.");

        public static readonly Error TooShort = Error.Validation(
            "UserName.TooShort", "User name is too short.");
    }
    
    public static class UserPassword
    {
        public static readonly Error Empty = Error.Validation(
            "UserPassword.Empty", "User password is empty.");

        public static readonly Error TooLong = Error.Validation(
            "UserPassword.TooLong", "User password is too long.");

        public static readonly Error TooShort = Error.Validation(
            "UserPassword.TooShort", "User password is too short.");
    }
    public static class RoleName
    {
        public static readonly Error IsTheSame = Error.Validation(
            "RoleName.IsTheSame", "It is impossible to change the name to the same.");

        public static readonly Error Empty = Error.Validation(
            "RoleName.Empty", "Role name is empty.");
    }

    public static class Comment
    {
        public static readonly Error IsTheSame = Error.Validation(
            "Comment.IsTheSame", "It is impossible to change the comment to the same.");
    }
    
    public static class Image
    {
        public static readonly Error IsTheSame = Error.Validation(
            "Image.IsTheSame", "It is impossible to change the image to the same.");
    }
    public static class GameName
    {
        public static readonly Error IsTheSame = Error.Validation(
            "GameName.IsTheSame", "It is impossible to change the name to the same.");

        public static readonly Error Empty = Error.Validation(
            "GameName.Empty", "Game name is empty.");
    }
    
    public static class SeasonName
    {
        public static readonly Error IsTheSame = Error.Validation(
            "SeasonName.IsTheSame", "It is impossible to change the name to the same.");

        public static readonly Error Empty = Error.Validation(
            "SeasonName.Empty", "Season name is empty.");

        public static readonly Error TooLong = Error.Validation(
            "SeasonName.TooLong", "Season name is too long.");

        public static readonly Error TooShort = Error.Validation(
            "SeasonName.TooShort", "Season name is too short.");
    }
}