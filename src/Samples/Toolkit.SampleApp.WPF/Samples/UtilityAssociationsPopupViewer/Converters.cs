using Esri.ArcGISRuntime.Mapping.Popups;
using Esri.ArcGISRuntime.UtilityNetworks;
using Esri.Calcite.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Esri.ArcGISRuntime.Toolkit.Samples.PopupViewer;

internal class VisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is IReadOnlyList<UtilityAssociationsFilterResult> list)
        {
            return list.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        if (value is IEnumerable<UtilityAssociationsPopupElement> enumerable)
        {
            return enumerable.Any() ? Visibility.Visible : Visibility.Collapsed;
        }
        if (value is Visibility visibility)
        {
            if (parameter is string paramString && paramString.Equals("Inverse"))
                return visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            return visibility;
        }
        if (value is UtilityAssociation association)
        {
            if (parameter is string iconName)
            {
                if (iconName.Equals(nameof(CalciteIcon.ConnectionEndLeft)) && association.AssociationType == UtilityAssociationType.JunctionEdgeObjectConnectivityFromSide)
                    return Visibility.Visible;
                if (iconName.Equals(nameof(CalciteIcon.ConnectionEndRight)) && association.AssociationType == UtilityAssociationType.JunctionEdgeObjectConnectivityToSide)
                    return Visibility.Visible;
                if (iconName.Equals(nameof(CalciteIcon.ConnectionMiddle)) && association.AssociationType == UtilityAssociationType.Connectivity)
                    return Visibility.Visible;
            }
            if (association.AssociationType == UtilityAssociationType.JunctionEdgeObjectConnectivityFromSide ||
                association.AssociationType == UtilityAssociationType.JunctionEdgeObjectConnectivityToSide ||
                association.AssociationType == UtilityAssociationType.JunctionEdgeObjectConnectivityMidspan ||
                association.AssociationType == UtilityAssociationType.Connectivity)
            {
                return Visibility.Visible;
            }
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        // Implementation here
        return null;
    }
}

internal class StringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is UtilityAssociationGroupResult groupResult)
        {
            return groupResult.AssociationResults.Count > 0 ? $"{groupResult.AssociationResults.Count}" : string.Empty;
        }
        if (value is IReadOnlyList<UtilityAssociationResult> associationResultList)
        {
            return associationResultList.Count > 0 ? $"{associationResultList.Count}" : string.Empty;
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        // Implementation here
        return null;
    }
}
