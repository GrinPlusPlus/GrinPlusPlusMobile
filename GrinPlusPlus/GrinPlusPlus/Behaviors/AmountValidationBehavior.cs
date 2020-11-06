using System;
using Prism.Behaviors;
using Xamarin.Forms;

namespace GrinPlusPlus.Behaviors
{
    class AmountValidationBehavior : BehaviorBase<Entry>
    {
        public static readonly BindableProperty IsValidProperty =
            BindableProperty.Create(nameof(IsValid), typeof(bool), typeof(AmountValidationBehavior), false, BindingMode.OneWayToSource);

        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            set
            {
                SetValue(IsValidProperty, value);
                System.Diagnostics.Debug.WriteLine($"Is True being set to: {value} by the EmailValidationBehavior");
            }
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            AssociatedObject.TextChanged += OnTextChanged;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
        }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length == 0)
            {
                IsValid = false;
                return;
            }  
            var amount = e.NewTextValue;
            if (e.NewTextValue.Contains("."))
            {
                var parts = e.NewTextValue.Split('.');
                if (parts[1].Trim().Equals(""))
                {
                    IsValid = false;
                    return;
                }
            }
            IsValid = Convert.ToDouble(amount) > 0;
        }
    }
}
