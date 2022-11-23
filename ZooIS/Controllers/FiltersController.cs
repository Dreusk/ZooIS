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
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace ZooIS.Controllers
{
    internal static class Utils
    {
        public static string NormalizeMethod(string Method)
        {
            return "Http" + String.Concat(Method[0].ToString().ToUpper(), Method.ToLower().AsSpan(1)) + "Attribute";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static MethodInfo GetCurrentAction(ActionExecutingContext context)
        {
            string Method = NormalizeMethod(context.HttpContext.Request.Method);
            string ActionName = (string)context.HttpContext.Request.RouteValues["action"];
            MethodInfo? Action = context.Controller.GetType().GetMethods().Where(action => action.Name == ActionName).FirstOrDefault(method => method.GetCustomAttributes().FirstOrDefault(attribute => attribute.GetType().Name == Method) is not null);
            if (Action is null)
                throw new InvalidOperationException("Action not found");
            return Action;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static MethodInfo GetCurrentAction(ResultExecutingContext context)
        {
            string Method = NormalizeMethod(context.HttpContext.Request.Method);
            string ActionName = (string)context.HttpContext.Request.RouteValues["action"];
            MethodInfo? Action = context.Controller.GetType().GetMethods().Where(action => action.Name == ActionName).FirstOrDefault(method => method.GetCustomAttributes().FirstOrDefault(attribute => attribute.GetType().Name == Method) is not null);
            if (Action is null)
                throw new InvalidOperationException("Action not found");
            return Action;
        }

        private static readonly Dictionary<string, string> StandartTitles = new() {
                { "Index",  "" },
                { "Crud",   "Регистрация" },
                { "Show",   "Просмотр" },
                { "Delete", "Удаление" }};

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetActionTitle(MethodInfo Action, ActionExecutingContext context)
        {
            var ControllerDisplay = context.Controller.GetType().GetCustomAttribute<DisplayAttribute>();
            var ActionDisplay = Action.GetCustomAttribute<DisplayAttribute>();
            if (ActionDisplay is null && !StandartTitles.ContainsKey(Action.Name))
                throw new ArgumentException("Given non-standart action with no display attribute.");
            return String.Join(" - ", new List<string> {
                ActionDisplay is not null
                    ? (string)ActionDisplay.GetType().GetProperty("Name").GetValue(ActionDisplay)
                    : StandartTitles[Action.Name],
                ControllerDisplay is not null
                    ? (string)ControllerDisplay.GetType().GetProperty("Name").GetValue(ControllerDisplay)
                    : ""}
            .Where(s => s != ""));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetActionTitle(MethodInfo Action, ResultExecutingContext context)
        {
            var ControllerDisplay = context.Controller.GetType().GetCustomAttribute<DisplayAttribute>();
            var ActionDisplay = Action.GetCustomAttribute<DisplayAttribute>();
            if (ActionDisplay is null && !StandartTitles.ContainsKey(Action.Name))
                throw new ArgumentException("Given non-standart action with no display attribute.");
            return String.Join(" - ", new List<string> {
                ControllerDisplay is not null
                    ? (string)ControllerDisplay.GetType().GetProperty("Name").GetValue(ControllerDisplay)
                    : "",
                ActionDisplay is not null
                    ? (string)ActionDisplay.GetType().GetProperty("Name").GetValue(ActionDisplay)
                    : StandartTitles[Action.Name] }
             .Where(s => s != ""));
        }
    }

    public class LogFilter : IAsyncAlwaysRunResultFilter
    {
        private readonly ZooISContext _context;

        public LogFilter(IServiceProvider services)
        {
            _context = services.CreateScope()
                .ServiceProvider.GetRequiredService<ZooISContext>();
        }

        /// <summary>
        /// Buffer while user is in proccess of creating. Can it be done with finer eficacy?
        /// </summary>
        /// <param name="UserName"></param>
        private User GetUser(string UserName)
        {
            while (_context.Users.FirstOrDefault(e => e.UserName == UserName) is null)
                Thread.Sleep(1000);
            return _context.Users.FirstOrDefault(e => e.UserName == UserName);
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated ||
            (context.HttpContext.Request.RouteValues["area"] is not null) ||
            (context.Controller.GetType().GetCustomAttribute<Ignore>() is not null))
            {
                await next();
                return;
            }
            var Action = Utils.GetCurrentAction(context);
            if (Action.GetCustomAttribute<Ignore>() is not null)
            {
                await next();
                return;
            }
            _context.Logs.Add(new() {   
                ts = DateTime.Now,
                Action = ActionType.Enter,
                Url = context.HttpContext.Request.Path.Value,
                PageTitle = Utils.GetActionTitle(Action, context),
                UserId = GetUser(context.HttpContext.User.Claims.First(e => e.Type == ClaimTypes.Name).Value).Id});
            _ = _context.SaveChangesAsync();
            await next();
        }
    }

    public class TitleFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.RouteValues["area"] is not null ||
            (context.Controller.GetType().GetCustomAttribute<Ignore>() is not null))
                return;
            var Action = Utils.GetCurrentAction(context);
            if (Action.GetCustomAttribute<Ignore>() is not null)
                return;
            var ViewData = (ViewDataDictionary)context.Controller.GetType().GetProperty("ViewData").GetValue(context.Controller);
            ViewData["Title"] = Utils.GetActionTitle(Action, context);
        }

        public void OnActionExecuted(ActionExecutedContext context) { } //Do nothing on this action.
    }
}
