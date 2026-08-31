using Microsoft.AspNetCore.Mvc;

namespace WebAppEstimate.Controllers;

public class ErrorController : Controller
{
    [Route("Error/{statusCode}")]
    public ActionResult HttpStatusCodeHandler(int statusCode)
    {
        ViewBag.StatusCode = statusCode;

        switch (statusCode)
        {
            case 404:
                ViewBag.ErrorMessage =
                    "Запрашиваемая страница не найдена.";
                break;

            case 403:
                ViewBag.ErrorMessage =
                    "У вас нет доступа к этой странице.";
                break;

            default:
                ViewBag.ErrorMessage =
                    "При выполнении запроса произошла ошибка.";
                break;
        }

        return View("HttpError");
    }
}