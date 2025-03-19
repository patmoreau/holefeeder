import '../models/make_purchase.dart';
import 'base_provider.dart';

class TransactionsProvider extends BaseProvider {
  TransactionsProvider({required super.restClient});

  Future<String?> makePurchase(MakePurchase item) async {
    try {
      final result = await restClient.makePurchase(item);
      if (result.response.statusCode == 201) {
        return result.data;
      }
      throw Exception('Could not make the purchase');
    } catch (e) {
      throw Exception('Could not make the purchase');
    }
  }
}
