#pragma checksum "C:\Users\Floor\source\Repos2\Bytme\Views\Home\About.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "91d4d1f7682f42e353c989b68e611e1ddbd6fa5e"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_About), @"mvc.1.0.view", @"/Views/Home/About.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Home/About.cshtml", typeof(AspNetCore.Views_Home_About))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "C:\Users\Floor\source\Repos2\Bytme\Views\_ViewImports.cshtml"
using bytme;

#line default
#line hidden
#line 2 "C:\Users\Floor\source\Repos2\Bytme\Views\_ViewImports.cshtml"
using bytme.Models;

#line default
#line hidden
#line 6 "C:\Users\Floor\source\Repos2\Bytme\Views\Home\About.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"91d4d1f7682f42e353c989b68e611e1ddbd6fa5e", @"/Views/Home/About.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1581b317dff1c7c2f6ed79465f07e2ed572a77b5", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_About : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 1 "C:\Users\Floor\source\Repos2\Bytme\Views\Home\About.cshtml"
  
    ViewData["Title"] = "General Information";

#line default
#line hidden
            BeginContext(55, 4, true);
            WriteLiteral("<h3>");
            EndContext();
            BeginContext(60, 19, false);
#line 4 "C:\Users\Floor\source\Repos2\Bytme\Views\Home\About.cshtml"
Write(ViewData["Message"]);

#line default
#line hidden
            EndContext();
            BeginContext(79, 9, true);
            WriteLiteral("</h3>\r\n\r\n");
            EndContext();
            BeginContext(126, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            BeginContext(226, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 11 "C:\Users\Floor\source\Repos2\Bytme\Views\Home\About.cshtml"
 if (SignInManager.IsSignedIn(User))
{

#line default
#line hidden
            BeginContext(269, 41, true);
            WriteLiteral("    <h2 style=\"text-align:center;\">Hello ");
            EndContext();
            BeginContext(311, 29, false);
#line 13 "C:\Users\Floor\source\Repos2\Bytme\Views\Home\About.cshtml"
                                    Write(UserManager.GetUserName(User));

#line default
#line hidden
            EndContext();
            BeginContext(340, 29, true);
            WriteLiteral(", how can we help you?</h2>\r\n");
            EndContext();
#line 14 "C:\Users\Floor\source\Repos2\Bytme\Views\Home\About.cshtml"
}
else
{

#line default
#line hidden
            BeginContext(381, 69, true);
            WriteLiteral("    <h2 style=\"text-align:center;\">Hello, how can we help you?</h2>\r\n");
            EndContext();
#line 18 "C:\Users\Floor\source\Repos2\Bytme\Views\Home\About.cshtml"
}

#line default
#line hidden
            BeginContext(453, 12, true);
            WriteLiteral("\r\n<hr />\r\n\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public UserManager<IdentityUser> UserManager { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public SignInManager<IdentityUser> SignInManager { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
