﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MazeGenerator
{
	public class TextBoxBehavior
	{
		public static readonly DependencyProperty AlwaysScrollToEndProperty =
			DependencyProperty.RegisterAttached("AlwaysScrollToEnd", typeof(bool), typeof(TextBoxBehavior), new PropertyMetadata(false, AlwaysScrollToEndChanged));
		
		private static void AlwaysScrollToEndChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var tb = sender as TextBox;
			if (tb != null)
			{
				var alwaysScrollToEnd = (e.NewValue != null) && (bool)e.NewValue;
				if (alwaysScrollToEnd)
				{
					tb.ScrollToEnd();
					tb.TextChanged += TextChanged;
				}
				else
				{
					tb.TextChanged -= TextChanged;
				}
			}
			else
			{
				throw new InvalidOperationException("The attached AlwaysScrollToEnd property can only be applied to TextBox instances.");
			}
		}

		public static bool GetAlwaysScrollToEnd(TextBox textBox)
		{
			if (textBox == null)
			{
				throw new ArgumentNullException("textBox");
			}

			return (bool)textBox.GetValue(AlwaysScrollToEndProperty);
		}

		public static void SetAlwaysScrollToEnd(TextBox textBox, bool alwaysScrollToEnd)
		{
			if (textBox == null)
			{
				throw new ArgumentNullException("textBox");
			}

			textBox.SetValue(AlwaysScrollToEndProperty, alwaysScrollToEnd);
		}

		private static void TextChanged(object sender, TextChangedEventArgs e)
		{
			(sender as TextBox).ScrollToEnd();
		}
	}
}
