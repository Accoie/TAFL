#include <iostream>
#include <string>
#include <sstream>
#include <vector>
using namespace std;

void BubbleSort(vector<int>& arr)
{
    int n = arr.size();
    for (int i = 0; i < n - 1; i++)
    {
        for (int j = 0; j < n - i - 1; j++)
        {
            if (arr[j] > arr[j + 1])
            {
                int temp = arr[j];
                arr[j] = arr[j + 1];
                arr[j + 1] = temp;
            }
        }
    }
}

int main()
{
    cout << "your numbers:" << endl;
    string input;
    getline(cin, input);

    if (input.empty())
    {
        cout << "empty input" << endl;
        return 0;
    }

    vector<int> numbers;
    stringstream ss(input);
    string token;

    try
    {
        while (getline(ss, token, ' '))
        {
            if (!token.empty())
            {
                numbers.push_back(stoi(token));
            }
        }

        BubbleSort(numbers);

        cout << "sorted numbers: ";
        for (size_t i = 0; i < numbers.size(); i++)
        {
            cout << numbers[i];
            if (i != numbers.size() - 1)
                cout << " ";
        }
        cout << endl;
    }
    catch (const exception& e)
    {
        cout << "Error: numbers must be integers" << endl;
    }

    return 0;
}