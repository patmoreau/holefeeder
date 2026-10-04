import ExpoModulesCore
import ExpoUI
import SwiftUI
import UIKit

final class KeyboardDoneAccessory {
    static let shared = KeyboardDoneAccessory()

    var label = "Done"
    var identifier = "keyboard-done-button"
    private var observers: [NSObjectProtocol] = []

    func start() {
        guard observers.isEmpty else { return }
        let center = NotificationCenter.default
        observers = [
            center.addObserver(forName: UITextField.textDidBeginEditingNotification, object: nil, queue: .main) { [weak self] note in
                guard let field = note.object as? UITextField, Self.isNumeric(field.keyboardType) else { return }
                self?.attach(to: field)
            },
            center.addObserver(forName: UITextView.textDidBeginEditingNotification, object: nil, queue: .main) { [weak self] note in
                guard let view = note.object as? UITextView, Self.isNumeric(view.keyboardType) else { return }
                self?.attach(to: view)
            },
        ]
    }

    func stop() {
        observers.forEach(NotificationCenter.default.removeObserver)
        observers = []
    }

    private static func isNumeric(_ type: UIKeyboardType) -> Bool {
        [.decimalPad, .numberPad, .phonePad, .asciiCapableNumberPad].contains(type)
    }

    private func toolbar() -> UIToolbar {
        let bar = UIToolbar()
        bar.sizeToFit()
        let done = UIBarButtonItem(title: label, style: .done, target: self, action: #selector(dismiss))
        done.accessibilityIdentifier = identifier
        bar.items = [UIBarButtonItem(barButtonSystemItem: .flexibleSpace, target: nil, action: nil), done]
        return bar
    }

    private func attach(to field: UITextField) {
        field.inputAccessoryView = toolbar()
        field.reloadInputViews()
    }

    private func attach(to view: UITextView) {
        view.inputAccessoryView = toolbar()
        view.reloadInputViews()
    }

    @objc private func dismiss() {
        UIApplication.shared.sendAction(#selector(UIResponder.resignFirstResponder), to: nil, from: nil, for: nil)
    }
}

struct KeyboardDoneModifierView: ViewModifier {
    let label: String
    let identifier: String

    func body(content: Content) -> some View {
        content.onAppear {
            KeyboardDoneAccessory.shared.label = label
            KeyboardDoneAccessory.shared.identifier = identifier
        }
    }
}

public class KeyboardDoneModule: Module {
    public func definition() -> ModuleDefinition {
        Name("KeyboardDoneModule")

        OnCreate {
            KeyboardDoneAccessory.shared.start()
            ViewModifierRegistry.register("keyboardDoneButton") { params, _, _ in
                KeyboardDoneModifierView(
                    label: params["label"] as? String ?? "Done",
                    identifier: params["identifier"] as? String ?? "keyboard-done-button"
                )
            }
        }

        OnDestroy {
            KeyboardDoneAccessory.shared.stop()
            ViewModifierRegistry.unregister("keyboardDoneButton")
        }
    }
}
