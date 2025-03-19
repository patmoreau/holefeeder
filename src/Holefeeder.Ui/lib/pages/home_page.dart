import 'package:flutter/material.dart';
import 'package:holefeeder/helpers/constants.dart';
import 'package:holefeeder/pages/categories_view_model.dart';
import 'package:holefeeder/pages/dashboard_page.dart';
import 'package:holefeeder/pages/profile_page.dart';
import 'package:provider/provider.dart';

import 'categories_page.dart';

class HomePage extends StatefulWidget {
  final int initialIndex;
  const HomePage({required this.initialIndex, super.key});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  late int currentPageIndex;

  late List<Widget> pages;

  @override
  void initState() {
    super.initState();
    currentPageIndex = widget.initialIndex;
    pages = [DashboardPage(), CategoriesPage(Provider.of<CategoriesViewModel>(context, listen: false)), ProfilePage()];
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(holefeederTitle),
        foregroundColor: Colors.white,
      ),
      body: IndexedStack(
        index: currentPageIndex,
        children: pages,
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () {
          debugPrint('FAB pressed');
        },
        child: const Icon(Icons.add),
      ),
      bottomNavigationBar: NavigationBar(
        destinations: const [
          NavigationDestination(icon: Icon(Icons.home), label: 'Dashboard'),
          NavigationDestination(icon: Icon(Icons.category_outlined), label: 'Categories'),
          NavigationDestination(icon: Icon(Icons.person), label: 'Profile'),
        ],
        onDestinationSelected: (index) {
          setState(() {
            currentPageIndex = index;
          });
        },
        selectedIndex: currentPageIndex,
      ),
    );
  }
}
