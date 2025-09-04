using NUnit.Framework;
using PxGraf.Enums;
using PxGraf.Models.Queries;
using PxGraf.Models.SavedQueries;
using PxGraf.Models.SavedQueries.Versions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests.ModelTests
{
    [TestFixture]
    internal class SavedQueriesTests
    {
        private DateTime testDateTime;
        private MatrixQuery testQuery;
        private Layout testLayout;

        [SetUp]
        public void SetUp()
        {
            testDateTime = new DateTime(2023, 5, 15, 10, 30, 0, DateTimeKind.Utc);
            
            testQuery = new MatrixQuery
            {
                TableReference = new PxTableReference(["TestDB"], "test.px"),
                DimensionQueries = new Dictionary<string, DimensionQuery>
                {
                    { "Dimension1", new DimensionQuery() }
                }
            };
            
            testLayout = new Layout(
                ["Row1", "Row2"],
                ["Column1", "Column2"]
            );
        }

        #region SavedQueryV10 Tests

        [Test]
        public void SavedQueryV10_ToSavedQuery_ConvertsBasicProperties()
        {
            // Arrange
            SavedQueryV10 v10 = new()
            {
                Archived = true,
                CreationTime = testDateTime,
                Query = testQuery,
                Settings = new SavedQueryV10.VisualizationSettingsV10
                {
                    SelectedVisualization = VisualizationType.LineChart,
                    RowDimensionCodes = ["Row1", "Row2"],
                    ColumnDimensionCodes = ["Column1", "Column2"],
                    CutYAxis = true,
                    MultiselectableDimensionCode = "Dimension1"
                }
            };

            // Act
            SavedQuery result = v10.ToSavedQuery();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Archived, Is.EqualTo(true));
                Assert.That(result.CreationTime, Is.EqualTo(testDateTime));
                Assert.That(result.Query, Is.EqualTo(testQuery));
                Assert.That(result.Version, Is.EqualTo("1.0"));
                Assert.That(result.Draft, Is.EqualTo(false));
                Assert.That(result.Settings.VisualizationType, Is.EqualTo(VisualizationType.LineChart));
                Assert.That(result.Settings.CutYAxis, Is.EqualTo(true));
                Assert.That(result.Settings.MultiselectableDimensionCode, Is.EqualTo("Dimension1"));
                Assert.That(result.Settings.Layout, Is.Null); // Layout is not set in V1.0
            });
        }

        [Test]
        public void SavedQueryV10_ToSavedQuery_WithPivotRequested_AddsToLegacyProperties()
        {
            // Arrange
            SavedQueryV10 v10 = new SavedQueryV10
            {
                Settings = new SavedQueryV10.VisualizationSettingsV10
                {
                    SelectedVisualization = VisualizationType.Table,
                    PivotRequested = true
                }
            };

            // Act
            SavedQuery result = v10.ToSavedQuery();

            // Assert
            Assert.That(result.LegacyProperties.ContainsKey("PivotRequested"), Is.True);
            Assert.That(result.LegacyProperties["PivotRequested"], Is.EqualTo(true));
        }

        [TestCase(VisualizationType.LineChart)]
        [TestCase(VisualizationType.VerticalBarChart)]
        [TestCase(VisualizationType.GroupVerticalBarChart)]
        [TestCase(VisualizationType.StackedVerticalBarChart)]
        [TestCase(VisualizationType.PercentVerticalBarChart)]
        [TestCase(VisualizationType.HorizontalBarChart)]
        [TestCase(VisualizationType.GroupHorizontalBarChart)]
        [TestCase(VisualizationType.StackedHorizontalBarChart)]
        [TestCase(VisualizationType.PercentHorizontalBarChart)]
        [TestCase(VisualizationType.PieChart)]
        [TestCase(VisualizationType.PyramidChart)]
        [TestCase(VisualizationType.ScatterPlot)]
        [TestCase(VisualizationType.Table)]
        [TestCase(VisualizationType.KeyFigure)]
        public void SavedQueryV10_ToSavedQuery_CorrectlyHandlesAllVisualizationTypes(VisualizationType visualizationType)
        {
            // Arrange
            SavedQueryV10 v10 = new()
            {
                Settings = new SavedQueryV10.VisualizationSettingsV10
                {
                    SelectedVisualization = visualizationType,
                    RowDimensionCodes = ["Row1", "Row2"],
                    ColumnDimensionCodes = ["Column1", "Column2"],
                }
            };

            // Act
            SavedQuery result = v10.ToSavedQuery();

            // Assert
            Assert.That(result.Settings.VisualizationType, Is.EqualTo(visualizationType));
        }

        [Test]
        public void SavedQueryV10_ToSavedQuery_HandlesDefaultSelectableDimensionCodes()
        {
            // Arrange
            Dictionary<string, List<string>> defaultCodes = new()
            {
                { "Dim1", new List<string> { "Value1", "Value2" } },
                { "Dim2", new List<string> { "Value3" } }
            };

            SavedQueryV10 v10 = new()
            {
                Settings = new SavedQueryV10.VisualizationSettingsV10
                {
                    SelectedVisualization = VisualizationType.LineChart,
                    DefaultSelectableDimensionCodes = defaultCodes
                }
            };

            // Act
            SavedQuery result = v10.ToSavedQuery();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Settings.DefaultSelectableDimensionCodes, Is.Not.Null);
                Assert.That(result.Settings.DefaultSelectableDimensionCodes.Count, Is.EqualTo(2));
                Assert.That(result.Settings.DefaultSelectableDimensionCodes["Dim1"].Count, Is.EqualTo(2));
                Assert.That(result.Settings.DefaultSelectableDimensionCodes["Dim2"].Count, Is.EqualTo(1));
            });
        }

        #endregion

        #region SavedQueryV11 Tests

        [Test]
        public void SavedQueryV11_ToSavedQuery_ConvertsBasicProperties()
        {
            // Arrange
            SavedQueryV11 v11 = new()
            {
                Archived = true,
                CreationTime = testDateTime,
                Query = testQuery,
                Settings = new SavedQueryV11.VisualizationSettingsV11
                {
                    VisualizationType = VisualizationType.LineChart,
                    Layout = testLayout,
                    CutYAxis = true,
                    MultiselectableDimensionCode = "Dimension1",
                    ShowDataPoints = true
                }
            };

            // Act
            SavedQuery result = v11.ToSavedQuery();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Archived, Is.EqualTo(true));
                Assert.That(result.CreationTime, Is.EqualTo(testDateTime));
                Assert.That(result.Query, Is.EqualTo(testQuery));
                Assert.That(result.Version, Is.EqualTo("1.1"));
                Assert.That(result.Draft, Is.EqualTo(false));
                Assert.That(result.Settings.VisualizationType, Is.EqualTo(VisualizationType.LineChart));
                Assert.That(result.Settings.CutYAxis, Is.EqualTo(true));
                Assert.That(result.Settings.MultiselectableDimensionCode, Is.EqualTo("Dimension1"));
                Assert.That(result.Settings.Layout, Is.EqualTo(testLayout));
                Assert.That(result.Settings.ShowDataPoints, Is.EqualTo(true));
            });
        }

        [TestCase(VisualizationType.LineChart)]
        [TestCase(VisualizationType.VerticalBarChart)]
        [TestCase(VisualizationType.GroupVerticalBarChart)]
        [TestCase(VisualizationType.StackedVerticalBarChart)]
        [TestCase(VisualizationType.PercentVerticalBarChart)]
        [TestCase(VisualizationType.HorizontalBarChart)]
        [TestCase(VisualizationType.GroupHorizontalBarChart)]
        [TestCase(VisualizationType.StackedHorizontalBarChart)]
        [TestCase(VisualizationType.PercentHorizontalBarChart)]
        [TestCase(VisualizationType.PieChart)]
        [TestCase(VisualizationType.PyramidChart)]
        [TestCase(VisualizationType.ScatterPlot)]
        [TestCase(VisualizationType.Table)]
        [TestCase(VisualizationType.KeyFigure)]
        public void SavedQueryV11_ToSavedQuery_CorrectlyHandlesAllVisualizationTypes(VisualizationType visualizationType)
        {
            // Arrange
            SavedQueryV11 v11 = new()
            {
                Settings = new SavedQueryV11.VisualizationSettingsV11
                {
                    VisualizationType = visualizationType,
                    Layout = testLayout
                }
            };

            // Act
            SavedQuery result = v11.ToSavedQuery();

            // Assert
            Assert.That(result.Settings.VisualizationType, Is.EqualTo(visualizationType));
        }

        [Test]
        public void SavedQueryV11_ToSavedQuery_HandlesSpecificVisualizationTypeSettings()
        {
            // Arrange
            SavedQueryV11 v11 = new()
            {
                Settings = new SavedQueryV11.VisualizationSettingsV11
                {
                    VisualizationType = VisualizationType.ScatterPlot,
                    Layout = testLayout,
                    CutYAxis = true,
                    MarkerSize = 120
                }
            };

            // Act
            SavedQuery result = v11.ToSavedQuery();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Settings.VisualizationType, Is.EqualTo(VisualizationType.ScatterPlot));
                Assert.That(result.Settings.CutYAxis, Is.EqualTo(true));
                Assert.That(result.Settings.MarkerSize, Is.EqualTo(120));
            });
        }

        [Test]
        public void SavedQueryV11_FromSavedQuery_CopiesSettingsCorrectly()
        {
            // Arrange
            SavedQuery savedQuery = new()
            {
                Archived = true,
                CreationTime = testDateTime,
                Query = testQuery,
                Version = "1.1"
            };

            LineChartVisualizationSettings settings = new (testLayout, true, "Dimension1", null, true);
            savedQuery.Settings = settings;

            // Act
            SavedQueryV11 v11 = new (savedQuery);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(v11.Archived, Is.EqualTo(true));
                Assert.That(v11.CreationTime, Is.EqualTo(testDateTime));
                Assert.That(v11.Query, Is.EqualTo(testQuery));
                Assert.That(v11.Settings.VisualizationType, Is.EqualTo(VisualizationType.LineChart));
                Assert.That(v11.Settings.CutYAxis, Is.EqualTo(true));
                Assert.That(v11.Settings.MultiselectableDimensionCode, Is.EqualTo("Dimension1"));
                Assert.That(v11.Settings.Layout, Is.EqualTo(testLayout));
                Assert.That(v11.Settings.ShowDataPoints, Is.EqualTo(true));
            });
        }

        #endregion

        #region SavedQueryV12 Tests

        [Test]
        public void SavedQueryV12_ToSavedQuery_ConvertsBasicProperties()
        {
            // Arrange
            SavedQueryV12 v12 = new()
            {
                Archived = true,
                CreationTime = testDateTime,
                Query = testQuery,
                Draft = true,
                Settings = new SavedQueryV11.VisualizationSettingsV11
                {
                    VisualizationType = VisualizationType.LineChart,
                    Layout = testLayout,
                    CutYAxis = true,
                    MultiselectableDimensionCode = "Dimension1"
                }
            };

            // Act
            SavedQuery result = v12.ToSavedQuery();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Archived, Is.EqualTo(true));
                Assert.That(result.CreationTime, Is.EqualTo(testDateTime));
                Assert.That(result.Query, Is.EqualTo(testQuery));
                Assert.That(result.Version, Is.EqualTo("1.2"));
                Assert.That(result.Draft, Is.EqualTo(true));
                Assert.That(result.Settings.VisualizationType, Is.EqualTo(VisualizationType.LineChart));
                Assert.That(result.Settings.CutYAxis, Is.EqualTo(true));
                Assert.That(result.Settings.MultiselectableDimensionCode, Is.EqualTo("Dimension1"));
                Assert.That(result.Settings.Layout, Is.EqualTo(testLayout));
            });
        }

        [Test]
        public void SavedQueryV12_FromSavedQuery_CopiesSettingsAndDraftCorrectly()
        {
            // Arrange
            SavedQuery savedQuery = new()
            {
                Archived = true,
                CreationTime = testDateTime,
                Query = testQuery,
                Version = "1.2",
                Draft = true
            };

            LineChartVisualizationSettings settings = new(testLayout, true, "Dimension1", null, true);
            savedQuery.Settings = settings;

            // Act
            SavedQueryV12 v12 = new(savedQuery);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(v12.Archived, Is.EqualTo(true));
                Assert.That(v12.CreationTime, Is.EqualTo(testDateTime));
                Assert.That(v12.Query, Is.EqualTo(testQuery));
                Assert.That(v12.Draft, Is.EqualTo(true));
                Assert.That(v12.Settings.VisualizationType, Is.EqualTo(VisualizationType.LineChart));
                Assert.That(v12.Settings.CutYAxis, Is.EqualTo(true));
                Assert.That(v12.Settings.MultiselectableDimensionCode, Is.EqualTo("Dimension1"));
                Assert.That(v12.Settings.Layout, Is.EqualTo(testLayout));
                Assert.That(v12.Settings.ShowDataPoints, Is.EqualTo(true));
            });
        }

        [TestCase(true)]
        [TestCase(false)]
        public void SavedQueryV12_ToSavedQuery_PreservesDraftFlag(bool isDraft)
        {
            // Arrange
            SavedQueryV12 v12 = new()
            {
                Draft = isDraft,
                Settings = new SavedQueryV11.VisualizationSettingsV11
                {
                    VisualizationType = VisualizationType.LineChart,
                    Layout = testLayout
                }
            };

            // Act
            SavedQuery result = v12.ToSavedQuery();

            // Assert
            Assert.That(result.Draft, Is.EqualTo(isDraft));
        }

        #endregion

        #region Version Compatibility Tests

        [Test]
        public void SavedQuery_UpgradeFromV10_ToV11_PreservesAllSettings()
        {
            // Arrange - Create a V1.0 saved query
            var v10 = new SavedQueryV10
            {
                Archived = true,
                CreationTime = testDateTime,
                Query = testQuery,
                Settings = new SavedQueryV10.VisualizationSettingsV10
                {
                    SelectedVisualization = VisualizationType.LineChart,
                    RowDimensionCodes = ["Row1", "Row2"],
                    ColumnDimensionCodes = ["Column1", "Column2"],
                    CutYAxis = true,
                    MultiselectableDimensionCode = "Dimension1",
                    PivotRequested = true
                }
            };

            // Convert V1.0 to SavedQuery
            SavedQuery savedQuery = v10.ToSavedQuery();

            // Act - Convert SavedQuery to V1.1
            SavedQueryV11 v11 = new(savedQuery);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(v11.Archived, Is.EqualTo(true));
                Assert.That(v11.CreationTime, Is.EqualTo(testDateTime));
                Assert.That(v11.Query, Is.EqualTo(testQuery));
                Assert.That(v11.Settings.VisualizationType, Is.EqualTo(VisualizationType.LineChart));
                Assert.That(v11.Settings.CutYAxis, Is.EqualTo(true));
                Assert.That(v11.Settings.MultiselectableDimensionCode, Is.EqualTo("Dimension1"));
                Assert.That(v11.Settings.Layout, Is.Null); // Layout is not set in V1.0
            });
        }

        [Test]
        public void SavedQuery_UpgradeFromV11_ToV12_PreservesAllSettings()
        {
            // Arrange - Create a V1.1 saved query
            SavedQueryV11 v11 = new()
            {
                Archived = true,
                CreationTime = testDateTime,
                Query = testQuery,
                Settings = new SavedQueryV11.VisualizationSettingsV11
                {
                    VisualizationType = VisualizationType.LineChart,
                    Layout = testLayout,
                    CutYAxis = true,
                    MultiselectableDimensionCode = "Dimension1",
                    ShowDataPoints = true
                }
            };

            // Convert V1.1 to SavedQuery
            SavedQuery savedQuery = v11.ToSavedQuery();

            // Act - Convert SavedQuery to V1.2
            SavedQueryV12 v12 = new(savedQuery);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(v12.Archived, Is.EqualTo(true));
                Assert.That(v12.CreationTime, Is.EqualTo(testDateTime));
                Assert.That(v12.Query, Is.EqualTo(testQuery));
                Assert.That(v12.Settings.VisualizationType, Is.EqualTo(VisualizationType.LineChart));
                Assert.That(v12.Settings.CutYAxis, Is.EqualTo(true));
                Assert.That(v12.Settings.MultiselectableDimensionCode, Is.EqualTo("Dimension1"));
                Assert.That(v12.Settings.ShowDataPoints, Is.EqualTo(true));
                
                // Draft should be false by default when upgrading from V1.1
                Assert.That(v12.Draft, Is.EqualTo(false));
            });
        }

        #endregion
    }
}
