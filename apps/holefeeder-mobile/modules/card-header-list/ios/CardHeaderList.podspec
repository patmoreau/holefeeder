Pod::Spec.new do |s|
  s.name           = 'CardHeaderList'
  s.version        = '1.0.0'
  s.summary        = 'A native SwiftUI List with collapsible header scroll tracking'
  s.description    = 'Provides a SwiftUI List that emits scroll offset events for animated collapsible headers'
  s.author         = ''
  s.homepage       = 'https://docs.expo.dev/modules/'
  s.platforms      = {
    :ios => '15.1',
    :tvos => '15.1'
  }
  s.source         = { git: '' }
  s.static_framework = true

  s.dependency 'ExpoModulesCore'

  s.pod_target_xcconfig = {
    'DEFINES_MODULE' => 'YES',
  }

  s.source_files = "**/*.{h,m,mm,swift,hpp,cpp}"
end
