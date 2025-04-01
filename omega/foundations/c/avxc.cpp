#include "pch.h"
#include "avxc.h"
#if __has_include("avxc.g.cpp")
#include "avxc.g.cpp"
#endif

using namespace winrt;
using namespace Windows::UI::Xaml;

namespace winrt::avxc::implementation
{
    int32_t avxc::MyProperty()
    {
        throw hresult_not_implemented();
    }

    void avxc::MyProperty(int32_t /* value */)
    {
        throw hresult_not_implemented();
    }

    void avxc::ClickHandler(IInspectable const&, RoutedEventArgs const&)
    {
        Button().Content(box_value(L"Clicked"));
    }
}
