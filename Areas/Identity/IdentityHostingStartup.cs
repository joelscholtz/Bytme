using System;
using bytme.Data;
using bytme.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(bytme.Areas.Identity.IdentityHostingStartup))]
namespace bytme.Areas.Identity
{
    public class IdentityHostingStartup
    {
        
    }
}