import 'package:holefeeder/core/models/tag.dart';

import 'base_provider.dart';

class TagsProvider extends BaseProvider {
  TagsProvider({required super.restClient});

  Future<List<Tag>> getTags() async {
    try {
      final result = await restClient.getTags();
      if (result.response.statusCode == 200) {
        return result.data;
      }
      throw Exception('Could not get tags');
    } catch (e) {
      throw Exception('Could not get tags');
    }
  }
}
