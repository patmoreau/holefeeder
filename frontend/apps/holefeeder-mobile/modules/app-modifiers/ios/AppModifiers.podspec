Pod::Spec.new do |s|
  s.name           = 'AppModifiers'
  s.version        = '1.0.0'
  s.summary        = 'Custom SwiftUI view modifiers for Holefeeder'
  s.description    = 'Registers scroll geometry and scroll transition modifiers with @expo/ui'
  s.author         = ''
  s.homepage       = 'https://docs.expo.dev/modules/'
  s.platforms      = {
    :ios => '15.1',
    :tvos => '15.1'
  }
  s.source         = { git: '' }
  s.static_framework = true

  s.dependency 'ExpoModulesCore'
  s.dependency 'ExpoUI'

  # Swift/Objective-C compatibility
  s.pod_target_xcconfig = {
    'DEFINES_MODULE' => 'YES',
  }

  s.source_files = "**/*.{h,m,mm,swift,hpp,cpp}"
end
