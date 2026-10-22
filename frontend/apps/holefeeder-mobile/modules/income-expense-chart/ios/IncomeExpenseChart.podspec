Pod::Spec.new do |s|
  s.name           = 'IncomeExpenseChart'
  s.version        = '1.0.0'
  s.summary        = 'Income vs expenses line chart for Insights'
  s.description    = 'Swift Charts line chart comparing income and expenses per budget period'
  s.author         = ''
  s.homepage       = 'https://docs.expo.dev/modules/'
  s.platforms      = {
    :ios => '16.4'
  }
  s.source         = { git: '' }
  s.static_framework = true

  s.dependency 'ExpoModulesCore'

  s.pod_target_xcconfig = {
    'DEFINES_MODULE' => 'YES',
  }

  s.source_files = "**/*.{h,m,mm,swift,hpp,cpp}"
end
