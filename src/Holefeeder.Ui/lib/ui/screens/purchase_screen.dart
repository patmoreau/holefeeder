import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../core/models/category.dart';
import '../../core/view_models/screens/purchase_screen_view_model.dart';
import '../shared/view_model_provider.dart';

class PurchaseScreen extends StatefulWidget {
  const PurchaseScreen({super.key});

  @override
  State<PurchaseScreen> createState() => _PurchaseScreenState();
}

class _PurchaseScreenState extends State<PurchaseScreen> {
  final GlobalKey<FormState> _formKey = GlobalKey<FormState>();
  bool isSwitched = false;
  bool isChecked = false;
  String? selectedCategory;

  @override
  Widget build(BuildContext context) {
    return ViewModelProvider<PurchaseViewModel>(
      model: PurchaseViewModel(context: context),
      builder: (PurchaseViewModel model) {
        return Form(
          key: _formKey,
          child: PopScope(
            onPopInvokedWithResult: (_, _) async {
              var router = GoRouter.of(context);
              if (router.canPop()) {
                router.pop();
              } else {
                router.go('/');
              }
            },
            child: _buildScaffold(model),
          ),
        );
      },
    );
  }

  Widget _buildScaffold(PurchaseViewModel model) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Purchase'),
        foregroundColor: Colors.white,
      ),
      body: SingleChildScrollView(
        child: Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            children: [
              TextFormField(
                decoration: const InputDecoration(labelText: 'Amount'),
                keyboardType: TextInputType.number,
                initialValue: model.amount.toString(),
                onChanged: (value) {
                  model.updateAmount(double.tryParse(value) ?? 0.0);
                },
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Please enter an amount';
                  }
                  final doubleValue = double.tryParse(value);
                  if (doubleValue == null || doubleValue < 0) {
                    return 'Please enter a valid amount';
                  }
                  return null;
                },
              ),
              FutureBuilder<List<Category>>(
                future: model.categoriesFuture,
                builder: (context, snapshot) {
                  if (snapshot.connectionState == ConnectionState.waiting) {
                    return CircularProgressIndicator();
                  } else if (snapshot.hasError) {
                    return Text('Error: ${snapshot.error}');
                  } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
                    return Text('No categories available');
                  } else {
                    final categories =
                        snapshot.data!.map((Category category) {
                          return DropdownMenuItem<String>(
                            value: category.id,
                            child: Text(category.name),
                          );
                        }).toList();
                    return DropdownButton<String>(
                      value: selectedCategory,
                      hint: const Text('Select a category'),
                      items: categories,
                      onChanged: (String? newValue) {
                        setState(() {
                          selectedCategory = newValue;
                        });
                      },
                    );
                  }
                },
              ),
              Padding(
                padding: const EdgeInsets.symmetric(vertical: 16.0),
                child: ElevatedButton(
                  onPressed: () {
                    if (_formKey.currentState?.validate() ?? false) {
                      model
                          .makePurchase()
                          .then((_) {
                            ScaffoldMessenger.of(context).showSnackBar(
                              const SnackBar(
                                content: Text('Purchase successful'),
                              ),
                            );
                            GoRouter.of(context).go('/');
                          })
                          .catchError((error) {
                            ScaffoldMessenger.of(context).showSnackBar(
                              SnackBar(content: Text('Error: $error')),
                            );
                          });
                    } else {
                      ScaffoldMessenger.of(context).showSnackBar(
                        const SnackBar(
                          content: Text('Please fill in all fields'),
                        ),
                      );
                    }
                  },
                  child: const Text('Purchase'),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
