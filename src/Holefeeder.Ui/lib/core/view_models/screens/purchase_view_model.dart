import 'package:decimal/decimal.dart';
import 'package:holefeeder/core/models/account.dart';
import 'package:holefeeder/core/models/category.dart';
import 'package:holefeeder/core/models/make_purchase.dart';
import 'package:holefeeder/core/providers/accounts_provider.dart';
import 'package:holefeeder/core/providers/categories_provider.dart';
import 'package:holefeeder/core/providers/tags_provider.dart';
import 'package:holefeeder/core/providers/transactions_provider.dart';
import 'package:holefeeder/core/view_models/base_form_state.dart';
import 'package:provider/provider.dart';

import '../../enums/date_interval_type_enum.dart';
import '../base_view_model.dart';

class PurchaseFormState extends BaseFormState {
  final Decimal amount;
  final DateTime date;
  final String note;
  final Account? selectedAccount;
  final Category? selectedCategory;
  final List<String> tags;
  final bool isCashflow;
  final DateTime effectiveDate;
  final DateIntervalType intervalType;
  final int frequency;
  final int recurrence;

  PurchaseFormState({
    Decimal? amount,
    DateTime? date,
    this.note = '',
    this.selectedAccount,
    this.selectedCategory,
    this.tags = const [],
    this.isCashflow = false,
    DateTime? effectiveDate,
    this.intervalType = DateIntervalType.monthly,
    this.frequency = 1,
    this.recurrence = 0,
    super.state = ViewFormState.initial,
    super.errorMessage,
  }) : date = date ?? DateTime.now(),
       amount = amount ?? Decimal.zero,
       effectiveDate = effectiveDate ?? DateTime.now();

  PurchaseFormState copyWith({
    Decimal? amount,
    DateTime? date,
    String? note,
    Account? selectedAccount,
    Category? selectedCategory,
    List<String>? tags,
    bool? isCashflow,
    DateTime? effectiveDate,
    DateIntervalType? intervalType,
    int? frequency,
    int? recurrence,
    ViewFormState? state,
    String? errorMessage,
  }) {
    return PurchaseFormState(
      amount: amount ?? this.amount,
      date: date ?? this.date,
      note: note ?? this.note,
      selectedAccount: selectedAccount ?? this.selectedAccount,
      selectedCategory: selectedCategory ?? this.selectedCategory,
      tags: tags ?? this.tags,
      isCashflow: isCashflow ?? this.isCashflow,
      effectiveDate: effectiveDate ?? this.effectiveDate,
      intervalType: intervalType ?? this.intervalType,
      frequency: frequency ?? this.frequency,
      recurrence: recurrence ?? this.recurrence,
      state: state ?? this.state,
      errorMessage: errorMessage ?? this.errorMessage,
    );
  }
}

class PurchaseViewModel extends BaseViewModel {
  final AccountsProvider _accountsProvider;
  final CategoriesProvider _categoriesProvider;
  final TagsProvider _tagsProvider;
  final TransactionsProvider _transactionsProvider;

  PurchaseFormState _formState = PurchaseFormState();

  PurchaseFormState get formState => _formState;

  List<Account> _accounts = [];

  List<Account> get accounts => _accounts;
  List<Category> _categories = [];

  List<Category> get categories => _categories;
  List<String> _tags = [];

  List<String> get tags => _tags;

  PurchaseViewModel({
    required super.context,
    AccountsProvider? accountsProvider,
    CategoriesProvider? categoriesProvider,
    TagsProvider? tagsProvider,
    TransactionsProvider? transactionsProvider,
  }) : _accountsProvider = accountsProvider ?? Provider.of<AccountsProvider>(context, listen: false),
       _categoriesProvider = categoriesProvider ?? Provider.of<CategoriesProvider>(context, listen: false),
       _tagsProvider = tagsProvider ?? Provider.of<TagsProvider>(context, listen: false),
       _transactionsProvider = transactionsProvider ?? Provider.of<TransactionsProvider>(context, listen: false) {
    loadInitialData();
  }

  Future<void> loadInitialData() async {
    _updateState((s) => s.copyWith(state: ViewFormState.loading));

    try {
      _accounts = await _accountsProvider.getAccounts();
      _categories = await _categoriesProvider.getCategories();
      _tags = (await _tagsProvider.getTags()).map((t) => t.tag).toList();

      _updateState(
        (s) => s.copyWith(
          state: ViewFormState.ready,
          selectedAccount: _accounts.firstOrNull,
          selectedCategory: _categories.firstOrNull,
        ),
      );
    } catch (e) {
      _updateState((s) => s.copyWith(state: ViewFormState.error, errorMessage: "Failed to load data: $e"));
    }
  }

  void updateAmount(Decimal value) => _updateState((s) => s.copyWith(amount: value));

  void updateDate(DateTime value) => _updateState((s) => s.copyWith(date: value));

  void updateNote(String value) => _updateState((s) => s.copyWith(note: value));

  void setSelectedAccount(Account? account) => _updateState((s) => s.copyWith(selectedAccount: account));

  void setSelectedCategory(Category? category) => _updateState((s) => s.copyWith(selectedCategory: category));

  void updateTags(List<String> tags) => _updateState((s) => s.copyWith(tags: tags));

  void updateIsCashflow(bool value) => _updateState((s) => s.copyWith(isCashflow: value));

  void updateEffectiveDate(DateTime value) => _updateState((s) => s.copyWith(effectiveDate: value));

  void updateIntervalType(DateIntervalType? value) {
    if (value != null) {
      _updateState((s) => s.copyWith(intervalType: value));
    }
  }

  void updateFrequency(int value) => _updateState((s) => s.copyWith(frequency: value));

  void updateRecurrence(int value) => _updateState((s) => s.copyWith(recurrence: value));

  bool validate() {
    if (_formState.amount <= Decimal.zero) return false;
    if (_formState.selectedAccount == null) return false;
    if (_formState.selectedCategory == null) return false;
    return true;
  }

  Future<void> makePurchase() async {
    await _transactionsProvider.makePurchase(
      MakePurchase(
        amount: _formState.amount,
        description: _formState.note,
        accountId: _formState.selectedAccount!.id,
        categoryId: _formState.selectedCategory!.id,
        tags: _formState.tags.toList(),
        date: _formState.date,
        cashflow:
            _formState.isCashflow
                ? CashflowRequest(
                  effectiveDate: _formState.effectiveDate,
                  intervalType: _formState.intervalType,
                  frequency: _formState.frequency,
                  recurrence: _formState.recurrence,
                )
                : null,
      ),
    );
  }

  void _updateState(PurchaseFormState Function(PurchaseFormState) update) {
    _formState = update(_formState);
    notifyListeners();
  }
}
