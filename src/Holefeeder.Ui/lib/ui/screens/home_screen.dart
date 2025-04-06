import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:holefeeder/l10n/l10n.dart';
import 'package:holefeeder/ui/screens/dashboard_screen.dart';
import 'package:holefeeder/ui/screens/profile_screen.dart';
import 'package:universal_platform/universal_platform.dart';

import 'categories_screen.dart';

class HomeScreen extends StatefulWidget {
  final int initialIndex;

  const HomeScreen({required this.initialIndex, super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  late int currentPageIndex;

  late List<Widget> pages;

  @override
  void initState() {
    super.initState();
    currentPageIndex = widget.initialIndex;
    pages = [DashboardScreen(), CategoriesScreen(), ProfileScreen()];
  }

  @override
  Widget build(BuildContext context) {
    return UniversalPlatform.isApple
        ? _buildForCupertino(context)
        : _buildForMaterial(context);
  }

  Widget _buildForCupertino(BuildContext context) => CupertinoTabScaffold(
    tabBar: CupertinoTabBar(
      items: const [
        BottomNavigationBarItem(
          icon: Icon(CupertinoIcons.home),
          label: 'Dashboard',
        ),
        BottomNavigationBarItem(
          icon: Icon(CupertinoIcons.paperplane_fill),
          label: 'Categories',
        ),
        BottomNavigationBarItem(
          icon: Icon(CupertinoIcons.person),
          label: 'Profile',
        ),
      ],
      onTap: (index) {
        setState(() {
          currentPageIndex = index;
        });
      },
    ),
    tabBuilder: (context, index) {
      return pages[index];
    },
  );

  Widget _buildForMaterial(BuildContext context) => Scaffold(
    appBar: AppBar(
      title: Text(AppLocalizations.of(context).holefeederTitle),
      foregroundColor: Colors.white,
    ),
    body: IndexedStack(index: currentPageIndex, children: pages),
    floatingActionButton: FloatingActionButton(
      onPressed: () {
        GoRouter.of(context).push('/purchase');
      },
      child: const Icon(Icons.add),
    ),
    bottomNavigationBar: NavigationBar(
      destinations: const [
        NavigationDestination(icon: Icon(Icons.home), label: 'Dashboard'),
        NavigationDestination(
          icon: Icon(Icons.category_outlined),
          label: 'Categories',
        ),
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
