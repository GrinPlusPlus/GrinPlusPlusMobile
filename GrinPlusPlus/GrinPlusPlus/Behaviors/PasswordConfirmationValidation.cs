using Prism.Behaviors;
using Xamarin.Forms;

namespace GrinPlusPlus.Behaviors
{
    class PasswordConfirmationValidation : BehaviorBase<Entry>
    {
        public string CompareWith
        {
            get { return (string)GetValue(CompareWithProperty); }
            set
            {
                SetValue(CompareWithProperty, value);
            }
        }

        public static readonly BindableProperty CompareWithProperty =
            BindableProperty.Create(nameof(CompareWith), typeof(string), typeof(PasswordConfirmationValidation), null);

        public static readonly BindableProperty IsValidProperty =
            BindableProperty.Create(nameof(IsValid), typeof(bool), typeof(PasswordConfirmationValidation), false, BindingMode.OneWayToSource);

        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            set
            {
                SetValue(IsValidProperty, value);
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
            if (e.NewTextValue == null || e.NewTextValue.Length == 0)
            {
                IsValid = false;
                return;
            }

            string password = e.NewTextValue;

            IsValid = !string.IsNullOrEmpty(password) && password.Length >= 8 && password.Equals(CompareWith);
        }
    }
}
