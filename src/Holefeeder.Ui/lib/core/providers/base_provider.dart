import 'package:holefeeder/core/utils/rest_client.dart';

abstract class BaseProvider {
  RestClient restClient;

  BaseProvider({required this.restClient});

  void dispose() {}
}
