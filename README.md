SimpleEventTypeProvider
===============
Sample Code is FSC: Error FS2024: Static linking may not use assembly that targets different profile.

# Problem

1. Build SimpleEventTypeProvider project.
2. Build SimpleEventTypeLocalLib project.

```F#
namespace SimpleEventTypeLocalLib

type MainPage = Moonmile.SimpleEventTypeProvider.XAML<"MainPage.xaml">

type MainPageEx(target:MainPage) =
    let mutable Name = ""
   
    do
       Name <- target.Name
       target.Button.Click |> Event.add( fun e -> ())  

       // Xamarin.Forms.Core が動的ロードされているので、静的リンクエラーになる
       // target.XButton.Clicked |> Event.add( fun e -> ())  // FS2024 error

       let pcl = target.XmlPCL.Xaml
       target.XmlPCL.Click |> Event.add( fun e -> ())

       let xpcl = target.XmlXamarinPCL
       xpcl.Text <- "test"
       // この時点で、Xamarin.Forms の型が参照されて静的リンクエラーになる

       xpcl.Clicked |> Event.add( fun e ->()) // FS2024 Error

       let btn = new Xamarin.Forms.Button()
       ()
```

# Note

![Alt fig](/doc/image1.png)


TypeProvider is Native Library for Windows. But Xamarin.Forms is PCL, Is it inconsistent NETCore / mscorlib in need ?

TypeProvider が Native Library なのだが、Xamarin.Forms が PCL なので、必要としている NETCore/mscorlib の不整合なのか。


# Reference 

Static linking PCL assembly with mscorlib reference · Issue #224 · fsharp/fsharp
https://github.com/fsharp/fsharp/issues/224

Xamarin.Forms の TypeProvider を作ろうとしたが断念したの巻 | Moonmile Solutions Blog
http://www.moonmile.net/blog/archives/6104



