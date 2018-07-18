﻿//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2017-2018  Dapplo
// 
//  For more information see: http://dapplo.net/
//  Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
//  This file is part of Dapplo.Windows
// 
//  Dapplo.Windows is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Dapplo.Windows is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have a copy of the GNU Lesser General Public License
//  along with Dapplo.Windows. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

using System.Collections.Generic;
using System.Linq;
using Dapplo.Windows.Input.Enums;

namespace Dapplo.Windows.Input.Keyboard
{
    /// <summary>
    /// This defines a certain key request which needs to be fullfilled
    /// </summary>
    public class KeyCombinationHandler : IKeyboardHookEventHandler
    {
        private readonly IList<VirtualKeyCode> _otherPressedKeys = new List<VirtualKeyCode>();
        private readonly bool[] _availableKeys;
        private readonly VirtualKeyCode[] _wantedCombination;

        /// <summary>
        /// Get the VirtualKeyCodes which trigger the combination
        /// </summary>
        public VirtualKeyCode[] TriggerCombination => _wantedCombination;

        /// <summary>
        /// Defines if the key press needs to be passed through to other applications.
        /// By default (false) a keypress which is specified is marked as handled and will not be seen by others
        /// </summary>
        public bool IsPassthrough { get; set; }

        /// <summary>
        /// Create a KeyCombinationHandler for the specified VirtualKeyCodes
        /// </summary>
        /// <param name="keyCombination">IEnumerable with VirtualKeyCodes</param>
        public KeyCombinationHandler(IEnumerable<VirtualKeyCode> keyCombination)
        {
            _wantedCombination = keyCombination.Distinct().ToArray();
            _availableKeys = new bool[_wantedCombination.Length];
        }

        /// <summary>
        /// Create a KeyCombinationHandler for the specified VirtualKeyCodes
        /// </summary>
        /// <param name="keyCombination">params with VirtualKeyCodes</param>
        public KeyCombinationHandler(params VirtualKeyCode[] keyCombination)
        {
            _wantedCombination = keyCombination.Distinct().ToArray();
            _availableKeys = new bool[_wantedCombination.Length];
        }

        /// <summary>
        /// Check if the keys are pressed
        /// </summary>
        /// <param name="keyboardHookEventArgs">KeyboardHookEventArgs</param>
        public bool Handle(KeyboardHookEventArgs keyboardHookEventArgs)
        {
            bool keyMatched = false;
            for (int i = 0; i < _wantedCombination.Length; i++)
            {
                if (!CompareVk(keyboardHookEventArgs.Key, _wantedCombination[i]))
                {
                    continue;
                }

                _availableKeys[i] = keyboardHookEventArgs.IsKeyDown;
                keyMatched = true;
                break;
            }

            if (!keyMatched)
            {
                if (keyboardHookEventArgs.IsKeyDown)
                {
                    _otherPressedKeys.Add(keyboardHookEventArgs.Key);
                }
                else
                {
                    _otherPressedKeys.Remove(keyboardHookEventArgs.Key);
                }
            }

            // Make as handled
            var isHandled = _otherPressedKeys.Count == 0 && _availableKeys.All(b => b);
            if (isHandled && !IsPassthrough)
            {
                keyboardHookEventArgs.Handled = true;
            }
            return isHandled;
        }

        /// <summary>
        /// Helper method to compare VirtualKeyCode
        /// </summary>
        /// <param name="current">VirtualKeyCode</param>
        /// <param name="expected">VirtualKeyCode</param>
        /// <returns>bool true if match</returns>
        private static bool CompareVk(VirtualKeyCode current, VirtualKeyCode expected)
        {
            if (current == expected)
            {
                return true;
            }

            switch (expected)
            {
                case VirtualKeyCode.Shift:
                    return VirtualKeyCode.LeftShift == current || VirtualKeyCode.RightShift == current;
                case VirtualKeyCode.Control:
                    return VirtualKeyCode.LeftControl == current || VirtualKeyCode.RightControl == current;
                case VirtualKeyCode.Menu:
                    return VirtualKeyCode.LeftMenu == current || VirtualKeyCode.RightMenu == current;
            }

            return false;
        }
    }
}