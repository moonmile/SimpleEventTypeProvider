namespace XamlPcl

type XamlPage() = 
    let event1 = new Event<_>()

    member this.X = "F#"
    member val Xaml = "Xaml code" with get, set

    [<CLIEvent>]
    member this.Click = event1.Publish
    member this.ClickEvent(arg) =
        event1.Trigger(this, arg)    
