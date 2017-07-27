using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorButton : MonoBehaviour
{
	public GameAttribute AttributeToChange;
	Slider _slider;
	InputField _inputField;

	private void OnEnable() {
		if (!_slider || !_inputField) {
			_slider = GetComponentInChildren<Slider>();
			_inputField = GetComponentInChildren<InputField>();
		}
		StartCoroutine("DelayedUpdateItself");

	}

	IEnumerator DelayedUpdateItself() {
		yield return null;
		if (_slider && _inputField) {
			MoreValue();
			MinusValue();
		}
	}

	private void Start() {
		_slider = GetComponentInChildren<Slider>();
		_inputField = GetComponentInChildren<InputField>();

	}

	public void MoreValue() {
		int newValue = EditorManager._.AddAttributePoint(AttributeToChange);
		_slider.value = newValue;
		_inputField.text = newValue.ToString();
	}

	public void MinusValue() {
		int newValue = EditorManager._.RemoveAttributePoint(AttributeToChange);
		_slider.value = newValue;
		_inputField.text = newValue.ToString();

	}

	public void SliderHolding(Slider value) {
		if (value != null) {
			if (_slider == null) {
				_slider = value;
			} else {
				value.value = (int)value.value;
				EditorManager._.SetAttributePoint(AttributeToChange, (int)value.value);
			}
			_inputField.text = value.value.ToString();
		}
	}

	public void InputValueChanged(InputField value) {
		if (value != null) {
			if (_inputField == null) {
				_inputField = value;
			} else {
				if (value.text != "0") {
					EditorManager._.SetAttributePoint(AttributeToChange, System.Int32.Parse(value.text));
				}
			}
			_slider.value = System.Int32.Parse(value.text);
		}
	}

	public void InputValueEnter(InputField value) {
		if (value != null) {
			if (_inputField == null) {
				_inputField = value;
			} else {
				if (value.text != "0") {
					EditorManager._.SetAttributePoint(AttributeToChange, System.Int32.Parse(value.text));
				}
			}
			_slider.value = System.Int32.Parse(value.text);
		}
	}

}
