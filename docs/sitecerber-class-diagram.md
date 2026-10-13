# UML class diagram — SiteCerber

Ниже представлена UML class diagram для модуля `SiteCerber`, который отвечает за параметрическое создание фланца, генерацию 3D-модели, чертежа и заполнение параметров через GigaChat.

```mermaid
classDiagram
direction LR

namespace SiteCerber.Controllers {
    class HomeController {
        +Index() IActionResult
    }
}

namespace SiteCerber.Components {
    class FlangePage {
        -FlangeModel Model
        -string PromptText
        -string Message
        -string ResultPath
        -string PreviewUrl
        -string DrawingUrl
        +GenerateAsync() Task
        +FillWithAiAsync() Task
        +DownloadStlAsync() Task
        +DownloadSvgAsync() Task
    }
}

namespace SiteCerber.Models {
    class FlangeModel {
        +double OuterDiameter
        +double InnerDiameter
        +double Thickness
        +int HoleCount
        +double HoleDiameter
        +double BoltCircleDiameter
    }

    class FlangeModelValidator {
        +Validate(FlangeModel model)
    }
}

namespace SiteCerber.Services {
    class IOpenScadService {
        <<interface>>
        +GenerateFlangeAsync(FlangeModel model) Task~OpenScadResult~
    }

    class OpenScadService {
        +GenerateFlangeAsync(FlangeModel model) Task~OpenScadResult~
    }

    class IFlangeDrawingService {
        <<interface>>
        +GenerateDrawing(FlangeModel model) string
    }

    class FlangeDrawingService {
        +GenerateDrawing(FlangeModel model) string
    }

    class IGigaChatService {
        <<interface>>
        +ParseFlangeAsync(string promptText) Task~FlangeModel~
    }

    class GigaChatService {
        +ParseFlangeAsync(string promptText) Task~FlangeModel~
    }

    class OpenScadResult {
        +bool Success
        +string ScadPath
        +string StlPath
        +string PreviewPath
        +string ErrorMessage
    }
}

HomeController --> FlangePage : открывает страницу

FlangePage *-- FlangeModel : содержит
FlangePage ..> FlangeModelValidator : проверяет
FlangePage ..> IOpenScadService : генерирует 3D-модель
FlangePage ..> IFlangeDrawingService : генерирует SVG-чертёж
FlangePage ..> IGigaChatService : заполняет параметры по описанию

OpenScadService ..|> IOpenScadService
FlangeDrawingService ..|> IFlangeDrawingService
GigaChatService ..|> IGigaChatService

OpenScadService ..> OpenScadResult : возвращает
FlangeDrawingService ..> FlangeModel : использует
OpenScadService ..> FlangeModel : использует
GigaChatService ..> FlangeModel : создаёт/заполняет
FlangeModelValidator ..> FlangeModel : валидирует
```
