using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;

namespace App
{
    [Activity(Label = "@string/calculator_label")]
    public class CalculatorActivity : Activity
    {
        private static Func<int, int, int> DefaultOperation = (o1, o2) => o1;

        private int firstOperand;
        private int secondOperand;
        private Func<int, int, int> operation;

        private TextView displayTextView;
        private TextView errorTextView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Calculator);

            displayTextView = FindViewById<TextView>(Resource.Id.displayTextView);
            displayTextView.TextChanged += OnDisplayTextViewChanged;

            FindViewById<Button>(Resource.Id.button0).Click += OnNumberButtonClick;
            FindViewById<Button>(Resource.Id.button1).Click += OnNumberButtonClick;
            FindViewById<Button>(Resource.Id.button2).Click += OnNumberButtonClick;
            FindViewById<Button>(Resource.Id.button3).Click += OnNumberButtonClick;
            FindViewById<Button>(Resource.Id.button4).Click += OnNumberButtonClick;
            FindViewById<Button>(Resource.Id.button5).Click += OnNumberButtonClick;
            FindViewById<Button>(Resource.Id.button6).Click += OnNumberButtonClick;
            FindViewById<Button>(Resource.Id.button7).Click += OnNumberButtonClick;
            FindViewById<Button>(Resource.Id.button8).Click += OnNumberButtonClick;
            FindViewById<Button>(Resource.Id.button9).Click += OnNumberButtonClick;

            FindViewById<Button>(Resource.Id.divideButton).Click += OnOperationButtonClick;
            FindViewById<Button>(Resource.Id.multiplyButton).Click += OnOperationButtonClick;
            FindViewById<Button>(Resource.Id.minusButton).Click += OnOperationButtonClick;
            FindViewById<Button>(Resource.Id.plusButton).Click += OnOperationButtonClick;

            FindViewById<Button>(Resource.Id.equalityButton).Click += OnEqualityButtonClick;
            FindViewById<Button>(Resource.Id.clearButton).Click += OnClearButtonClick;

            errorTextView = FindViewById<TextView>(Resource.Id.errorTextView);

            this.InitState();
        }

        private void InitState(int operand1 = 0, int operand2 = 0)
        {
            this.firstOperand = operand1;
            this.secondOperand = operand2;
            this.operation = DefaultOperation;

            this.displayTextView.Text = this.firstOperand.ToString();
        }

        public void OnDisplayTextViewChanged(object sender, TextChangedEventArgs e)
        {
            string inputText = ((TextView)sender).Text;
            if (!string.IsNullOrEmpty(inputText))
            {
                int operand = Convert.ToInt32(inputText);
                if (this.operation == null || this.operation == DefaultOperation)
                {
                    this.firstOperand = operand;
                }
                else
                {
                    this.secondOperand = operand;
                }
            }
        }

        private void OnNumberButtonClick(object sender, EventArgs e)
        {
            this.displayTextView.Text = ((Button)sender).Text;
            this.ClearErrors();
        }

        private void OnOperationButtonClick(object sender, EventArgs e)
        {
            string selectedOperation = ((Button)sender).Text;

            switch (selectedOperation)
            {
                case "/":
                    this.operation = (first, second) =>
                    {
                        if (second == 0)
                        {
                            throw new InvalidOperationException("It is impossible to divide by ZERO.");
                        }

                        return first / second;
                    };
                    break;
                case "*":
                    this.operation = (first, second) => first * second;
                    break;
                case "-":
                    this.operation = (first, second) => first - second;
                    break;
                case "+":
                default:
                    this.operation = (first, second) => first + second;
                    break;
            }

            this.ClearErrors();
        }

        private void OnEqualityButtonClick(object sender, EventArgs e)
        {
            try
            {
                int answer = this.operation(this.firstOperand, this.secondOperand);

                InitState(answer);
            }
            catch (InvalidOperationException ex)
            {
                this.errorTextView.Text = ex.Message;
            }
        }

        private void OnClearButtonClick(object sender, EventArgs e)
        {
            InitState();
        }

        private void ClearErrors()
        {
            this.errorTextView.Text = string.Empty;
        }
    }
}