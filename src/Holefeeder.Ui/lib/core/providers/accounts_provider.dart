import 'package:holefeeder/core/models/account.dart';
import 'package:holefeeder/core/providers/base_provider.dart';

class AccountsProvider extends BaseProvider {
  AccountsProvider({required super.restClient});

  Future<List<Account>> getAccounts() async {
    try {
      final result = await restClient.getAccounts(['-favorite', 'name'], ['inactive:eq:false']);
      if (result.response.statusCode == 200) {
        return result.data;
      }
      throw Exception('Could not get the Accounts');
    } catch (e) {
      throw Exception('Could not get the Accounts');
    }
  }

  Future<Account> getAccount(String id) async {
    try {
      final result = await restClient.getAccount(id);
      if (result.response.statusCode == 200) {
        return result.data;
      }
      if (result.response.statusCode == 404) {
        throw Exception('Account not found');
      }
      throw Exception('Could not get the account');
    } catch (e) {
      throw Exception('Could not get the account');
    }
  }
}
