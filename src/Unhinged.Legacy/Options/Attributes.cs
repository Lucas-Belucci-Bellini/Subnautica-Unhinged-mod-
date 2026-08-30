using System;

namespace SMLHelper.V2.Options.Attributes
{
    // ⚠️ ATENÇÃO: estes atributos hoje são APENAS DE DECLARAÇÃO.
    //
    // Eles fazem o código legado COMPILAR, mas ainda NÃO constroem o painel de opções:
    // no SMLHelper eles eram lidos por reflexão pelo `OptionsPanelHandler`, e ligar isso
    // ao `Nautilus.Options` é trabalho à parte, ainda não feito.
    //
    // Um mod portado agora terá suas opções ignoradas em jogo — sem erro. Isto está
    // registrado em docs/PORTE-LEGADO.md; não é para ser descoberto no jogo.

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MenuAttribute : Attribute
    {
        public string Name { get; }
        public MenuAttribute(string name) => Name = name;
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ToggleAttribute : Attribute
    {
        /// <summary>Posição no painel de opções.</summary>
        public int Order { get; set; }

        public string Label { get; set; }
        public string Tooltip { get; set; }
        public ToggleAttribute() { }
        public ToggleAttribute(string label) => Label = label;
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SliderAttribute : Attribute
    {
        /// <summary>Posição no painel de opções.</summary>
        public int Order { get; set; }

        public string Label { get; set; }
        public string Tooltip { get; set; }
        public float Min { get; set; }
        public float Max { get; set; }
        public float Step { get; set; }
        public float DefaultValue { get; set; }
        public string Format { get; set; }
        public SliderAttribute() { }
        public SliderAttribute(string label) => Label = label;
        public SliderAttribute(string label, float min, float max) { Label = label; Min = min; Max = max; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ChoiceAttribute : Attribute
    {
        /// <summary>Posição no painel de opções.</summary>
        public int Order { get; set; }

        public string Label { get; set; }
        public string Tooltip { get; set; }
        public string[] Options { get; set; }
        public ChoiceAttribute() { }
        public ChoiceAttribute(string label) => Label = label;
        public ChoiceAttribute(params string[] options) => Options = options;
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class KeybindAttribute : Attribute
    {
        /// <summary>Posição no painel de opções.</summary>
        public int Order { get; set; }

        public string Label { get; set; }
        public string Tooltip { get; set; }
        public KeybindAttribute() { }
        public KeybindAttribute(string label) => Label = label;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ButtonAttribute : Attribute
    {
        /// <summary>Posição no painel de opções.</summary>
        public int Order { get; set; }

        public string Label { get; set; }
        public string Tooltip { get; set; }
        public ButtonAttribute() { }
        public ButtonAttribute(string label) => Label = label;
    }

    /// <summary>Método chamado quando o valor muda.</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class OnChangeAttribute : Attribute
    {
        public string MethodName { get; }
        public OnChangeAttribute(string methodName) => MethodName = methodName;
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class OnGameObjectCreatedAttribute : Attribute
    {
        public string MethodName { get; }
        public OnGameObjectCreatedAttribute(string methodName) => MethodName = methodName;
    }
}

namespace SMLHelper.V2.Options
{
    /// <summary>Argumentos dos eventos de mudança do painel de opções legado.</summary>
    public class ConfigFileEventArgs : EventArgs
    {
        public object Value { get; }
        public ConfigFileEventArgs(object value) => Value = value;
    }

    public class ToggleChangedEventArgs : ConfigFileEventArgs
    {
        public new bool Value { get; }
        public ToggleChangedEventArgs(bool value) : base(value) => Value = value;
    }

    public class SliderChangedEventArgs : ConfigFileEventArgs
    {
        public new float Value { get; }
        public SliderChangedEventArgs(float value) : base(value) => Value = value;
    }

    public class ChoiceChangedEventArgs : ConfigFileEventArgs
    {
        public int Index { get; }
        public new string Value { get; }
        public ChoiceChangedEventArgs(int index, string value) : base(value) { Index = index; Value = value; }
    }

    public class KeybindChangedEventArgs : ConfigFileEventArgs
    {
        public UnityEngine.KeyCode Key { get; }
        public KeybindChangedEventArgs(UnityEngine.KeyCode key) : base(key) => Key = key;
    }
}
