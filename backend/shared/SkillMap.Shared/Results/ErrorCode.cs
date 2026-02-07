// <copyright file="ErrorCode.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SkillMap.Shared.Results;

public class ErrorCode
{
    // Prefixes
    public const string SYSTEMERRORPREFIX = "SE";
    public const string USERINPUTERRORPREFIX = "UIE";

    // System Errors
    public const string FAILEDTOSAVE = SYSTEMERRORPREFIX + "1"; // SE1
    public const string FAILEDTOCREATE = SYSTEMERRORPREFIX + "2"; // SE2
    public const string FAILEDTODELETE = SYSTEMERRORPREFIX + "3"; // SE3
    public const string FAILEDTOUPDATE = SYSTEMERRORPREFIX + "4"; // SE4
    public const string FAILEDTOGET = SYSTEMERRORPREFIX + "5"; // SE5
    public const string NOTFOUND = SYSTEMERRORPREFIX + "6"; // SE6
    public const string INTERNALERROR = SYSTEMERRORPREFIX + "7"; // SE7
    public const string TIMEOUT = SYSTEMERRORPREFIX + "8"; // SE8
    public const string UNAUTHORIZED = SYSTEMERRORPREFIX + "9"; // SE9
    public const string FORBIDDEN = SYSTEMERRORPREFIX + "10"; // SE10
    public const string CONFLICT = SYSTEMERRORPREFIX + "11"; // SE11

    // User Input Errors
    public const string VALIDATIONERROR = USERINPUTERRORPREFIX + "0"; // UIE0
    public const string INVALIDPASSWORD = USERINPUTERRORPREFIX + "1"; // UIE1
    public const string USERALREADYEXISTS = USERINPUTERRORPREFIX + "2"; // UIE2
    public const string USERNAMEREQUIRED = USERINPUTERRORPREFIX + "3"; // UIE3
    public const string EMAILINVALID = USERINPUTERRORPREFIX + "4"; // UIE4
    public const string PASSWORDINVALID = USERINPUTERRORPREFIX + "5"; // UIE5
    public const string ROLEREQUIRED = USERINPUTERRORPREFIX + "6"; // UIE6
    public const string ROLEINVALID = USERINPUTERRORPREFIX + "7"; // UIE7
    public const string EMAILREQUIRED = USERINPUTERRORPREFIX + "8"; // UIE8
    public const string PASSWORDREQUIRED = USERINPUTERRORPREFIX + "9"; // UIE9
    public const string INVALIDINPUT = USERINPUTERRORPREFIX + "10"; // UIE10

    // Email errors
    public const string EMAILSENDINGFAILED = SYSTEMERRORPREFIX + "12"; // SE12
    public const string INVALIDOREXPIREDTOKEN = USERINPUTERRORPREFIX + "11"; // UIE11
}