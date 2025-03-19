import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

class LearnFlutterPage extends StatefulWidget {
  const LearnFlutterPage({super.key});

  @override
  _LearnFlutterPageState createState() => _LearnFlutterPageState();
}

class _LearnFlutterPageState extends State<LearnFlutterPage> {
  bool isSwitched = false;
  bool isChecked = false;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        foregroundColor: Colors.white,
        title: const Text('Learn Flutter'),
        automaticallyImplyLeading: false,
        leading: IconButton(
          onPressed: () {
            var router = GoRouter.of(context);
            if(router.canPop()){
              router.pop();
            } else {
              router.go('/');
            }
          },
          icon: Icon(Icons.arrow_back_ios_new),
        ),
        actions: [
          IconButton(
            onPressed: () {
              debugPrint('IconButton pressed');
            },
            icon: const Icon(Icons.settings),
          ),
        ],
      ),
      body: SingleChildScrollView(
        child: Column(
          children: [
            Image.asset('assets/safe_256.png'),
            const SizedBox(height: 10),
            const Divider(color: Colors.black),
            Container(
              margin: const EdgeInsets.all(10.0),
              padding: const EdgeInsets.all(10.0),
              color: Colors.blueGrey,
              width: double.infinity,
              child: const Center(
                child: Text(
                  'This is a text widget',
                  style: TextStyle(color: Colors.white, fontSize: 20),
                ),
              ),
            ),
            ElevatedButton(
              style: ElevatedButton.styleFrom(
                backgroundColor: isSwitched ? Colors.blue : Colors.amber,
              ),
              onPressed: () {
                debugPrint('ElevatedButton pressed');
              },
              child: const Text('ElevatedButton'),
            ),
            OutlinedButton(
              onPressed: () {
                debugPrint('OutlinedButton pressed');
              },
              child: const Text('OutlinedButton'),
            ),
            TextButton(
              onPressed: () {
                debugPrint('TextButton pressed');
              },
              child: const Text('TextButton'),
            ),
            GestureDetector(
              behavior: HitTestBehavior.opaque,
              onTap: () {
                debugPrint('GestureDetector pressed');
              },
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                children: const [
                  Icon(Icons.local_fire_department, color: Colors.blue),
                  Text('Row widget'),
                  Icon(Icons.local_fire_department, color: Colors.blue),
                ],
              ),
            ),
            Switch(
              value: isSwitched,
              onChanged: (newValue) {
                setState(() {
                  isSwitched = newValue;
                });
              },
            ),
            Checkbox(
              value: isChecked,
              onChanged: (newValue) {
                setState(() {
                  isChecked = newValue!;
                });
              },
            ),
            Image.network('https://picsum.photos/250?image=9'),
          ],
        ),
      ),
    );
  }
}
