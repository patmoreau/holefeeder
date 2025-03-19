import 'package:decimal/decimal.dart';
import 'package:holefeeder/core/models/account.dart';
import 'package:holefeeder/core/models/category.dart';
import 'package:holefeeder/core/models/make_purchase.dart';
import 'package:holefeeder/core/providers/accounts_provider.dart';
import 'package:holefeeder/core/providers/categories_provider.dart';
import 'package:holefeeder/core/providers/transactions_provider.dart';
import 'package:provider/provider.dart';

import '../base_view_model.dart';

class PurchaseViewModel extends BaseViewModel {
  late AccountsProvider accountsProvider;
  late CategoriesProvider categoriesProvider;
  late TransactionsProvider transactionsProvider;

  late Future<List<Account>> _accountsFuture;
  late Future<List<Category>> _categoriesFuture;
  DateTime _date = DateTime.now();
  double _amount = 0.0;
  String _description = '';
  String _accountId = '';
  String _categoryId = '';
  List<String> _tags = [];

  Future<List<Account>> get accountsFuture => _accountsFuture;
  Future<List<Category>> get categoriesFuture => _categoriesFuture;

  double get amount => _amount;

  PurchaseViewModel({required super.context}) {
    accountsProvider = Provider.of<AccountsProvider>(context);
    categoriesProvider = Provider.of<CategoriesProvider>(context);
    transactionsProvider = Provider.of<TransactionsProvider>(context);

    _categoriesFuture = categoriesProvider.getCategories();
  }

  Future<void> refreshCategories() async {
    notifyListeners();
    _categoriesFuture = categoriesProvider.getCategories();
    notifyListeners();
    await _categoriesFuture;
  }

  void updateAmount(double value) {
    _amount = value;
    notifyListeners();
  }

  Future<void> makePurchase() async {
    if (_amount <= 0) {
      throw Exception('Amount must be greater than 0');
    }
    if (_accountId.isEmpty) {
      throw Exception('Account must be selected');
    }
    if (_categoryId.isEmpty) {
      throw Exception('Category must be selected');
    }

    await transactionsProvider.makePurchase(
      MakePurchase(
        amount: Decimal.parse(_amount.toString()),
        description: _description,
        accountId: _accountId,
        categoryId: _categoryId,
        tags: _tags,
        date: _date,
      ),
    );
  }
}
