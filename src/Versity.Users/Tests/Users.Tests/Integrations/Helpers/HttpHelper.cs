﻿namespace Users.Tests.Integrations.Helpers;

public static class HttpHelper
{
    public static string GetAllUsersUrl(int page) => "/api/users/" + page;
    public static string GetUserById(string guid) => "/api/users/" + guid;
    public static string ChangeUserPassword(string guid) => "/api/users/" + guid + "/password";
    public static string GiveAdminRoleToUser(string guid) => "/api/users/" + guid + "/set-admin";
    
    public static string Register() => "/api/auth/register";
}