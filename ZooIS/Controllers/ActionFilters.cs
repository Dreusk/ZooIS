using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Collections.Generic;
using ZooIS.Data;
using ZooIS.Models;

namespace ZooIS.Controllers
{
    internal static class Utils
    {
        public static string NormalizeMethod(string Method)
        {
            return "Http" + String.Concat(Method[0].ToString().ToUpper(), Method.ToLower().AsSpan(1)) + "Attribute";
        }

        public static MethodInfo? GetCurrentAction(ActionExecutingContext context)
        {
            string Method = NormalizeMethod(context.HttpContext.Request.Method);
            string ActionName = (string)context.HttpContext.Request.RouteValues["action"];
            MethodInfo? Action = context.Controller.GetType().GetMethods().Where(action => action.Name == ActionName).First(method => method.GetCustomAttributes().First(attribute => attribute.GetType().Name == Method) is not null);
            return Action;
        }

        public static MethodInfo? GetCurrentAction(ActionExecutedContext context)
        {
            string Method = NormalizeMethod(context.HttpContext.Request.Method);
            string ActionName = (string)context.HttpContext.Request.RouteValues["action"];
            MethodInfo? Action = context.Controller.GetType().GetMethods().Where(action => action.Name == ActionName).First(method => method.GetCustomAttributes().First(attribute => attribute.GetType().Name == Method) is not null);
            return Action;
        }

        public static string GetActionTitle(MethodInfo Action, ActionExecutingContext context)
        {
            Dictionary<string, string> StandartTitles = new() {
                { "Index"  , "" },
                { "Create" , "Регистрация" },
                { "Details", "Просмотр" },
                { "Edit"   , "Редакция" },
                { "Delete" , "Удаление" }};

            var ControllerDisplay = context.Controller.GetType().GetCustomAttribute<DisplayAttribute>();
            var ActionDisplay = Action.GetCustomAttribute<DisplayAttribute>();
            if (ActionDisplay is null && !StandartTitles.ContainsKey(Action.Name))
                throw new ArgumentException("Given non-standart action with no display attribute.");
            return ControllerDisplay is not null
                    ? (string)ControllerDisplay.GetType().GetProperty("Name").GetValue(ControllerDisplay) + " - "
                    : ""
                   + ActionDisplay is not null
                    ? (string)ActionDisplay.GetType().GetProperty("Name").GetValue(ActionDisplay)
                    : StandartTitles[Action.Name];
        }

        public static string GetActionTitle(MethodInfo Action, ActionExecutedContext context)
        {
            Dictionary<string, string> StandartTitles = new()
            {
                { "Index", "" },
                { "Create", "Регистрация" },
                { "Details", "Просмотр" },
                { "Edit", "Редакция" },
                { "Delete", "Удаление" }
            };

            var ActionTitle = Action.GetCustomAttribute(typeof(DisplayAttribute));
            if (ActionTitle is null && !StandartTitles.ContainsKey(Action.Name))
                throw new ArgumentException("Given is non-standart action with no display attribute.");
            return ActionTitle is not null
                    ? (string)ActionTitle.GetType().GetProperty("Name").GetValue(ActionTitle)
                    : StandartTitles[Action.Name];
        }
    }

    public class LogFilter : IActionFilter
    {
        private readonly ZooISContext _context;

        public LogFilter(ZooISContext context)
        {
            _context = context;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {

                _context.Logs.Add(new() {
                    ts = DateTime.Now,
                    Action = ActionType.Enter,
                    Url = context.HttpContext.Request.Path.Value,
                    PageTitle = Utils.GetActionTitle(Utils.GetCurrentAction(context), context),
                    UserId = context.HttpContext.User.Claims.First(e => e.Type == ClaimTypes.NameIdentifier).Value }); ;
                _context.SaveChanges();
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated) {
                _context.Logs.Add(new()
                {
                    ts = DateTime.Now,
                    Action = ActionType.Quit,
                    Url = context.HttpContext.Request.Path.Value,
                    PageTitle = Utils.GetActionTitle(Utils.GetCurrentAction(context), context),
                    UserId = context.HttpContext.User.Claims.First(e => e.Type == ClaimTypes.NameIdentifier).Value
                });
                _context.SaveChanges();
            }
        }
    }

    public class TitleFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var ViewData = (ViewDataDictionary)context.Controller.GetType().GetProperty("ViewData").GetValue(context.Controller);
            ViewData["Title"] = Utils.GetActionTitle(Utils.GetCurrentAction(context), context);
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
