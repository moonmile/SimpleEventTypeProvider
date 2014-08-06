namespace SimpleEventTypeLocalLib

type MainPage = Moonmile.SimpleEventTypeProvider.XAML<"MainPage.xaml">

type MainPageEx(target:MainPage) =
    let mutable Name = ""
    
    do
       Name <- target.Name
       target.Button.Click |> Event.add( fun e -> ())  
       // Xamarin.Forms.Core が動的ロードされているので、静的リンクエラーになる
       // target.XButton.Clicked |> Event.add( fun e -> ())  
       (*
       let col = target.XButton.BackgroundColor
       let a = col.A
       target.XButton.BackgroundColor <- Xamarin.Forms.Color.Black
       target.XButton.ChildAdded |> Event.add( fun e -> ())
       *)
       let pcl = target.XmlPCL.Xaml
       target.XmlPCL.Click |> Event.add( fun e -> ())

       let xpcl = target.XmlXamarinPCL
       xpcl.Text <- "test"
       // この時点で、Xamarin.Forms の型が参照されて静的リンクエラーになる
       xpcl.Clicked |> Event.add( fun e ->())

       let btn = new Xamarin.Forms.Button()
       ()
        