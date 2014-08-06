namespace Moonmile.FSharp.Lib
open System
open Microsoft.FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes
open System.Reflection

[<assembly:TypeProviderAssembly>]
do ()


type MyButton() =

    let event1 = new Event<_>()
    [<CLIEvent>]
    member this.Click = event1.Publish
    member this.ClickEvent(arg) =
        event1.Trigger(this, arg)    

[<TypeProvider>]
type SimpleEventType(config:TypeProviderConfig) as this = 
    inherit TypeProviderForNamespaces()
    let namespaceName = "Moonmile.SimpleEventTypeProvider" 
    let thisAssembly = Assembly.GetExecutingAssembly()


    /// 型生成を残す場合
    /// [<Litelal>]
    /// let xaml = "<ContentPage>...</ContentPage>"
    /// type MainPage = SimpleEventTypeProvider.XAML< xaml >
    // 型の定義
    let t = ProvidedTypeDefinition(thisAssembly, namespaceName, "XAML", Some(typeof<obj>), IsErased = false )
    do t.DefineStaticParameters(
        [ProvidedStaticParameter("xaml", typeof<string>)],
        fun typeName parameterValues -> 

            let outerType = 
                ProvidedTypeDefinition (thisAssembly, namespaceName, 
                    typeName, Some(typeof<obj>), IsErased = false )
            // テンポラリアセンブリに出力
            let tempAssembly = ProvidedAssembly(System.IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), ".dll"))
            tempAssembly.AddTypes <| [ outerType ]

            // コンストラクタの生成
            let ctor = ProvidedConstructor([], 
                            InvokeCode = fun args -> <@@ () @@> )
            do outerType.AddMember( ctor )

            // プロパティを追加
            let prop = 
                ProvidedProperty( "Name", typeof<string>, 
                    GetterCode = fun args -> <@@ "masuda tomoaki" @@> )
            do outerType.AddMember( prop )

            // ボタンを追加
            let propButotn = 
                ProvidedProperty( "Button", typeof<MyButton>, 
                    GetterCode = fun args -> 
                        <@@  
                            // let me = %%(args.[0]):obj
                            new MyButton()
                        @@> )
            do outerType.AddMember( propButotn )
            (*
            // 直接参照しても駄目
            let propXButton =
                ProvidedProperty( "XButton", typeof<Xamarin.Forms.Button>, 
                    GetterCode = fun args -> 
                        <@@  
                            // let me = %%(args.[0]):obj
                            new Xamarin.Forms.Button()
                        @@> )
            do outerType.AddMember( propXButton )
            *)


            let propXamlPcl =
                ProvidedProperty( "XmlPCL", typeof<XamlPcl.XamlPage>, 
                    GetterCode = fun args -> 
                        <@@  
                            // let me = %%(args.[0]):obj
                            new XamlPcl.XamlPage()
                        @@> )
            do outerType.AddMember( propXamlPcl )
            /// Xamarin.Forms 関連を PCL 外出しにしても駄目    
            let propXamarinPcl =
                ProvidedProperty( "XmlXamarinPCL", typeof< XamlPclXamarin.XamarinButton >, 
                    GetterCode = fun args -> 
                        <@@  
                            // let me = %%(args.[0]):obj
                            new XamlPclXamarin.XamarinButton()
                        @@> )
            do outerType.AddMember( propXamarinPcl )

            outerType
    )
    // 名前空間に型を追加
    do this.AddNamespace( namespaceName, [t] )

    // Xamarin.Forms.Core 用に追加
    override this.ResolveAssembly(args) = 
        let name = System.Reflection.AssemblyName(args.Name)
        let existingAssembly = 
            System.AppDomain.CurrentDomain.GetAssemblies()
            |> Seq.tryFind(fun a -> System.Reflection.AssemblyName.ReferenceMatchesDefinition(name, a.GetName()))
        match existingAssembly with
        | Some a -> a
        | None -> 
            // Fallback to default behavior
            base.ResolveAssembly(args)
