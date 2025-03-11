using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping.Popups;
using Esri.ArcGISRuntime.Portal;
using Esri.ArcGISRuntime.RealTime;
using Esri.ArcGISRuntime.Security;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using Esri.ArcGISRuntime.UtilityNetworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Esri.ArcGISRuntime.Toolkit.Samples.PopupViewer
{
    /// <summary>
    /// Interaction logic for UtilityAssociationsPopupViewerSample.xaml
    /// </summary>
    public partial class UtilityAssociationsPopupViewerSample : UserControl
    {
        public UtilityAssociationsPopupViewerSample()
        {
            InitializeComponent();

            AuthenticationManager.Current.ChallengeHandler = new ChallengeHandler(
                async (info) =>
                {
                    var credential = await AccessTokenCredential.CreateAsync(
                    new Uri("https://rt-nautilus115.esri.com/portal/sharing/rest"),
                            "publisher1",
                            "test.publisher01");
                    return credential;
                });

        }

        // Used in Callout to see feature details in PopupViewer
        private RuntimeImage InfoIcon { get; } = new RuntimeImage(new Uri("Samples/PopupViewer/info.png", UriKind.Relative));

        private async void mapView_GeoViewTapped(object sender, GeoViewInputEventArgs e)
        {
            try
            {
                var result = await mapView.IdentifyLayersAsync(e.Position, 3, false);

                // Retrieves or builds Popup from IdentifyLayerResult
                var popup = GetPopup(result) ?? BuildPopupFromGeoElement(result);
                if (popup != null)
                {
                    if (mapView.Map?.UtilityNetworks.FirstOrDefault() is UtilityNetwork utilityNetwork)
                    {
                        await utilityNetwork.LoadAsync();
                    }
                    await popup.EvaluateExpressionsAsync();
                    if (popup.EvaluatedElements.Any(p => p is UtilityAssociationsPopupElement))
                    {
                        AssociationsPopupElement.ItemsSource = popup.EvaluatedElements.OfType<UtilityAssociationsPopupElement>();
                    }
                    else
                    {
                        AssociationsPopupElement.ItemsSource = Enumerable.Empty<UtilityAssociationsPopupElement>();
                    }
                    popupViewer.Popup = popup;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private Popup GetPopup(IdentifyLayerResult result)
        {
            if (result == null)
            {
                return null;
            }

            var popup = result.Popups.FirstOrDefault();
            if (popup != null)
            {
                if (popup.GeoElement is DynamicEntityObservation deo)
                {
                    return new Popup(deo.GetDynamicEntity(), popup.PopupDefinition);
                }
                return popup;
            }

            return GetPopup(result.SublayerResults);
        }

        private Popup GetPopup(IEnumerable<IdentifyLayerResult> results)
        {
            if (results == null)
                return null;
            foreach (var result in results)
            {
                var popup = GetPopup(result);
                if (popup != null)
                    return popup;
            }

            return null;
        }

        private Popup BuildPopupFromGeoElement(IdentifyLayerResult result)
        {
            if (result == null)
                return null;
            var geoElement = result.GeoElements.FirstOrDefault();
            if (geoElement != null)
            {
                if (geoElement is DynamicEntityObservation obs)
                    geoElement = obs.GetDynamicEntity();
                if (result.LayerContent is IPopupSource source)
                {
                    var popupDefinition = source.PopupDefinition;
                    if (popupDefinition != null)
                    {
                        return new Popup(geoElement, popupDefinition);
                    }
                }

                return Popup.FromGeoElement(geoElement);
            }
            return BuildPopupFromGeoElement(result.SublayerResults);
        }

        private Popup BuildPopupFromGeoElement(IEnumerable<IdentifyLayerResult> results)
        {
            if (results == null)
            {
                return null;
            }
            foreach (var result in results)
            {
                var popup = BuildPopupFromGeoElement(result);
                if (popup != null)
                {
                    return popup;
                }
            }
            return null;
        }

        private void PopupBackground_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            popupViewer.Popup = null;
        }

        private async void popupViewer_PopupAttachmentClicked(object sender, UI.Controls.PopupAttachmentClickedEventArgs e)
        {
            if (!e.Attachment.IsLocal) // Attachment hasn't been downloaded
            {
                try
                {
                    // Make first click just load the attachment (or cancel a loading operation). Otherwise fallback to default behavior
                    if (e.Attachment.LoadStatus == LoadStatus.NotLoaded)
                    {
                        e.Handled = true;
                        await e.Attachment.LoadAsync();
                    }
                    else if (e.Attachment.LoadStatus == LoadStatus.FailedToLoad)
                    {
                        e.Handled = true;
                        await e.Attachment.RetryLoadAsync();
                    }
                    else if (e.Attachment.LoadStatus == LoadStatus.Loading)
                    {
                        e.Handled = true;
                        e.Attachment.CancelLoad();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to download attachment", ex.Message);
                }
            }
        }

        private void popupViewer_LinkClicked(object sender, UI.Controls.HyperlinkClickedEventArgs e)
        {
            // Include below line if you want to prevent the default action
            // e.Handled = true;

            // Perform custom action when a link is clicked
            Debug.WriteLine(e.Uri);
        }

        private void OnClosePopup(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void OnExitAssociations(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        }

        private void OnBack(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void OnFilterExpand(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GroupResultGrid.DataContext = AssociationsPopupElement.SelectedItem is UtilityAssociationsFilterResult filterResult ? filterResult : null;
            GroupResultGrid.Visibility = Visibility.Visible;

        }

        private void OnGroupCollapse(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void OnGroupExpand(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void OnAssociatedFeatureExpand(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}