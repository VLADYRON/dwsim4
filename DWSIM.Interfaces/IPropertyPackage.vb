﻿Public Interface IPropertyPackage

    Property UniqueID As String

    ReadOnly Property Name As String

    Property Tag As String

    Property CurrentMaterialStream As IMaterialStream

    Function CalculateEquilibrium(calctype As Enums.FlashCalculationType,
                                            val1 As Double, val2 As Double,
                                            mixmolefrac As Double(),
                                            initialKval As Double(),
                                            initialestimate As Double) As IFlashCalculationResult

End Interface