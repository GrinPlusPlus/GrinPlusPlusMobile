﻿using Prism.Behaviors;
using Xamarin.Forms;

namespace GrinPlusPlus.Behaviors
{
    class UsernameValidation : BehaviorBase<Entry>
    {
        public static readonly BindableProperty IsValidProperty =
            BindableProperty.Create(nameof(IsValid), typeof(bool), typeof(UsernameValidation), false, BindingMode.OneWayToSource);

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

            string username = e.NewTextValue;

            IsValid = !string.IsNullOrEmpty(username);
        }
    }
}
