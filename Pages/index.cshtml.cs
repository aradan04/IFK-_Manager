using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProyectoIFK.Pages;

public class IndexModel : PageModel
{
    public string CustomCSS { get; set; } = string.Empty;

    public void OnGet()
    {
        CustomCSS = @"
            body {
                margin: 0;
                background-color: #f4f4f4;
                display: flex;
                flex-direction: column;
                align-items: center;
                font-family: 'Segoe UI', sans-serif;
            }
            .top-banner {
                width: 100%;
                background-color: #1a1a1a;
                color: #d4af37;
                text-align: center;
                padding: 10px 0;
                font-weight: bold;
            }
            .login-container {
                margin-top: 30px;
                text-align: center;
                max-width: 450px;
            }
            .main-logo {
                width: 500px;
                margin-bottom: 40px;
            }
            label {
                display: block;
                font-weight: bold;
                margin-bottom: 10px;
                text-transform: uppercase;
            }
            input {
                width: 100%;
                padding: 12px;
                margin-bottom: 20px;
                border: 1px solid #ccc;
                border-radius: 5px;
                text-align: center;
            }
            .btn-login {
                background-color: #d4af37;
                color: #000;
                border: none;
                padding: 15px 40px;
                font-size: 1.2rem;
                font-weight: bold;
                border-radius: 8px;
                cursor: pointer;
                width: 100%;
            }
            .forgot-password {
                display: block;
                margin-top: 25px;
                color: #d4af37;
                text-decoration: none;
                font-style: italic;
            }";
    }

    public IActionResult OnPost()
    {
        return RedirectToPage("/inicio");
    }
}