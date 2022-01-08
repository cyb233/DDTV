﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Mime;

namespace DDTV_WEB_Server
{
    public class ProcessingControllerBase
    {
        [Produces(MediaTypeNames.Application.Json)]
        [ApiController]
        [Route("api/[controller]")]
        [AllowAnonymous]
        [Login]
        public class ApiControllerBase : ControllerBase
        {
           
        }
    }
    /// <summary>
    /// 对于DDTV_WEB_API权限验证和sig校验的筛选器操作
    /// </summary>
    public class LoginAttribute : ActionFilterAttribute
    {
        [HttpPost(Name = "Attribute")]
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.Method == "POST")
            {
                string sig = string.Empty;
                string time = string.Empty;
                string cmd = string.Empty;
                string accesskeyid = string.Empty;
                string B = filterContext.HttpContext.Request.ContentType;
                var T = filterContext.HttpContext.Request.Form;
                if (filterContext.HttpContext.Request.Form != null)
                {
                    sig = filterContext.HttpContext.Request.Form["sig"];
                    time = filterContext.HttpContext.Request.Form["time"];
                    cmd = filterContext.HttpContext.Request.Form["cmd"];
                    accesskeyid = filterContext.HttpContext.Request.Form["accesskeyid"];
                }
                if (!string.IsNullOrEmpty(sig))
                {
                    if (!string.IsNullOrEmpty(time) || !string.IsNullOrEmpty(cmd) || !string.IsNullOrEmpty(accesskeyid))
                    {
                        if (accesskeyid == RuntimeConfig.AccessKeyId)
                        {
                            Dictionary<string, string> parameters = new Dictionary<string, string>
                        {
                            { "accesskeyid", accesskeyid },
                            { "accesskeysecret", RuntimeConfig.AccessKeySecret },
                            { "cmd", cmd.ToLower() },
                            { "time", time }
                        };
                            string Original = string.Empty;
                            foreach (var item in parameters)
                            {
                                Original += $"{item.Key}={item.Value};";
                            }
                            string NewSig = DDTV_Core.SystemAssembly.EncryptionModule.Encryption.SHA1_Encrypt(Original);
                            if (NewSig != sig)
                            {
                                filterContext.HttpContext.Response.Redirect($"/api/AuthenticationFailed?code={MessageBase.code.APIAuthenticationFailed}&message=sig计算鉴权失败");
                                return;
                            }
                        }
                        else
                        {
                            filterContext.HttpContext.Response.Redirect($"/api/AuthenticationFailed?code={MessageBase.code.APIAuthenticationFailed}&message=sig计算鉴权失败");
                            return;
                        }
                    }
                    else
                    {
                        filterContext.HttpContext.Response.Redirect($"/api/AuthenticationFailed?code={MessageBase.code.APIAuthenticationFailed}&message=sig计算鉴权失败");
                        return;
                    }
                }
                else if (filterContext.HttpContext.Request.Cookies != null)
                {
                    string cookis = filterContext.HttpContext.Request.Cookies["DDTVUser"];
                    if (string.IsNullOrEmpty(cookis))
                    {
                        filterContext.HttpContext.Response.Cookies.Append("DDTVUser", "");
                        filterContext.HttpContext.Response.Redirect("/api/LoginErrer");
                        return;
                    }
                    else
                    {
                        if (cookis != RuntimeConfig.Cookis)
                        {
                            filterContext.HttpContext.Response.Cookies.Append("DDTVUser", "");
                            filterContext.HttpContext.Response.Redirect("/api/LoginErrer");
                            return;
                        }
                        else
                        {
                            //这里应该是进行sig校验的地方
                        }
                    }
                }
                else
                {
                    filterContext.HttpContext.Response.Cookies.Append("DDTVUser", "");
                    filterContext.HttpContext.Response.Redirect("/api/LoginErrer");
                    return;
                }
            }
        }
    }
}
