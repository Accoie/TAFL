#include <iostream>
#include <string>
using namespace std;

string ReverseString(const string& input)
{
    string result;

    for (int i = input.length() - 1; i >= 0; i--)
    {
        result += input[i];
    }

    return result;
}

int main()
{
    cout << "your string:" << endl;
    string input;
    getline(cin, input);

    if (input.empty())
    {
        cout << "empty input" << endl;
        return 0;
    }

    string reversed = ReverseString(input);
    cout << "reverse string: " << reversed << endl;

    return 0;
}