#pragma checksum "C:\Users\Cormac\source\repos\c0m\Hipparu\Components\DragNDrop.razor" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "d881eb0f028566a4dadbe979115e473f93c0f259"
// <auto-generated/>
#pragma warning disable 1591
namespace Hipparu.Components
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#nullable restore
#line 1 "C:\Users\Cormac\source\repos\c0m\Hipparu\_Imports.razor"
using System.Net.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\Cormac\source\repos\c0m\Hipparu\_Imports.razor"
using System.Net.Http.Json;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\Cormac\source\repos\c0m\Hipparu\_Imports.razor"
using Microsoft.AspNetCore.Components.Forms;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\Cormac\source\repos\c0m\Hipparu\_Imports.razor"
using Microsoft.AspNetCore.Components.Routing;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "C:\Users\Cormac\source\repos\c0m\Hipparu\_Imports.razor"
using Microsoft.AspNetCore.Components.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "C:\Users\Cormac\source\repos\c0m\Hipparu\_Imports.razor"
using Microsoft.AspNetCore.Components.WebAssembly.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "C:\Users\Cormac\source\repos\c0m\Hipparu\_Imports.razor"
using Microsoft.JSInterop;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "C:\Users\Cormac\source\repos\c0m\Hipparu\_Imports.razor"
using Hipparu;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "C:\Users\Cormac\source\repos\c0m\Hipparu\_Imports.razor"
using Hipparu.Shared;

#line default
#line hidden
#nullable disable
#nullable restore
#line 1 "C:\Users\Cormac\source\repos\c0m\Hipparu\Components\DragNDrop.razor"
using Hipparu.Data;

#line default
#line hidden
#nullable disable
    public partial class DragNDrop : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
            __builder.OpenElement(0, "div");
            __builder.AddAttribute(1, "class", true);
            __builder.AddAttribute(2, "style", "border: 2px solid black;");
            __builder.OpenElement(3, "button");
            __builder.AddAttribute(4, "onclick", Microsoft.AspNetCore.Components.EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, 
#nullable restore
#line 4 "C:\Users\Cormac\source\repos\c0m\Hipparu\Components\DragNDrop.razor"
                        ()=> OnRemoveClick.InvokeAsync(Item)

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(5, "class", "btn btn-secondary m-1");
            __builder.AddMarkupContent(6, "<span class=\"oi oi-trash\"></span>");
            __builder.CloseElement();
            __builder.AddMarkupContent(7, "\r\n    <span class=\"ml-3 oi oi-sun\"></span>");
            __builder.CloseElement();
        }
        #pragma warning restore 1998
#nullable restore
#line 14 "C:\Users\Cormac\source\repos\c0m\Hipparu\Components\DragNDrop.razor"
        [Parameter]
    public AnswerItem Item { get; set; }

    [Parameter]
    public EventCallback<AnswerItem> OnRemoveClick { get; set; } 

#line default
#line hidden
#nullable disable
    }
}
#pragma warning restore 1591
