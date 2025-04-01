#pragma once

#include "avxc.g.h"

namespace winrt::avxc::implementation
{
    struct avxc : avxcT<avxc>
    {
        avxc() 
        {
            // Xaml objects should not call InitializeComponent during construction.
            // See https://github.com/microsoft/cppwinrt/tree/master/nuget#initializecomponent
        }

        int32_t MyProperty();
        void MyProperty(int32_t value);

        void ClickHandler(Windows::Foundation::IInspectable const& sender, Windows::UI::Xaml::RoutedEventArgs const& args);
    };
}

namespace winrt::avxc::factory_implementation
{
    struct avxc : avxcT<avxc, implementation::avxc>
    {
    };
}
