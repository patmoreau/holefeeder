import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:holefeeder/ui/shared/view_model_provider.dart';
import 'package:holefeeder/ui/shared/widgets.dart';
import 'package:universal_platform/universal_platform.dart';

import '../../core/models/category.dart';
import '../../core/view_models/screens/categories_view_model.dart';

class CategoriesScreen extends StatefulWidget {
  const CategoriesScreen({super.key});

  @override
  State<CategoriesScreen> createState() => _CategoriesScreenState();
}

class _CategoriesScreenState extends State<CategoriesScreen> {
  @override
  Widget build(BuildContext context) =>
      UniversalPlatform.isApple
          ? _buildForCupertino(context)
          : _buildForMaterial(context);

  Widget _buildForCupertino(BuildContext context) => CupertinoPageScaffold(
    navigationBar: CupertinoNavigationBar(middle: const Text('Categories')),
    child: _buildScreen(context),
  );

  Widget _buildForMaterial(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Categories')),
      body: _buildScreen(context),
    );
  }

  Widget _buildScreen(
    BuildContext context,
  ) => ViewModelProvider<CategoriesViewModel>(
    model: CategoriesViewModel(context: context),
    builder: (CategoriesViewModel model) {
      return FutureBuilder<List<Category>>(
        future: model.categoriesFuture,
        builder: (_, AsyncSnapshot<List<Category>> snapshot) {
          switch (snapshot.connectionState) {
            case ConnectionState.done:
              if (snapshot.hasError) {
                return Padding(
                  padding: const EdgeInsets.all(16),
                  child: Center(
                    child: Column(
                      children: <Widget>[
                        const Text('Oops we had trouble loading your wishlist'),
                        const SizedBox(height: 32),
                        HolefeederWidgets.button(
                          onPressed: () async {
                            await model.refreshCategories();
                          },
                          child: const Text('Retry'),
                        ),
                      ],
                    ),
                  ),
                );
              }
              final List<Category> items = snapshot.data ?? <Category>[];
              if (items.isEmpty) {
                return const Center(
                  child: Text('Your wishlist is empty. Why not add some items'),
                );
              }
              return ListView.builder(
                itemCount: items.length,
                itemBuilder: (_, int index) => _buildRow(items[index]),
              );
            default:
              return Padding(
                padding: const EdgeInsets.all(16),
                child: Center(
                  child: Column(
                    children: <Widget>[
                      Text('Loading your wishlist'),
                      SizedBox(height: 32),
                      HolefeederWidgets.activityIndicator(),
                    ],
                  ),
                ),
              );
          }
        },
      );
    },
  );

  Widget _buildRow(Category item) {
    return UniversalPlatform.isApple
        ? CupertinoListTile(
          title: Text(item.name),
          subtitle: Text(item.color),
          trailing: IconButton(
            onPressed: () async {},
            icon: Icon(HolefeederWidgets.iconEdit),
          ),
        )
        : Card(
          child: Padding(
            padding: const EdgeInsets.all(8),
            child: Row(
              children: <Widget>[
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: <Widget>[Text(item.name), Text(item.color)],
                  ),
                ),
                IconButton(
                  onPressed: () async {},
                  icon: Icon(HolefeederWidgets.iconEdit),
                ),
                IconButton(
                  onPressed: () async {},
                  icon: Icon(HolefeederWidgets.iconDelete),
                ),
              ],
            ),
          ),
        );
  }
}
