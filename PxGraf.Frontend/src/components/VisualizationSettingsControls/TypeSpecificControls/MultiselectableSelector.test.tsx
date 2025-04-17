import React from 'react';
import '@testing-library/jest-dom';
import { render, screen } from '@testing-library/react';
import { MultiselectableSelector } from './MultiselectableSelector';
import { IDimension, EDimensionType } from "types/cubeMeta";
import { IVisualizationSettings } from '../../../types/visualizationSettings';
import { IVisualizationRules } from '../../../types/visualizationRules';

const mockDimensions: IDimension[] = [
	{
		code: 'cVar',
		name: { fi: 'cVarName' },
		type: EDimensionType.Content,
		values: [
			{
				code: 'cVal',
				name: { fi: 'cValName' },
				isVirtual: false
			}
		]
	},
	{
		code: 'tVar',
		name: { fi: 'tVarName' },
		type: EDimensionType.Time,
		values: [
			{
				code: 'tVal',
				name: { fi: 'tValName' },
				isVirtual: false
			}
		]
	},
	{
		code: 'msVar',
		name: { fi: 'msVarAName' },
		type: EDimensionType.Other,
		values: [
			{
				code: 'msVal1',
				name: { fi: 'msVal1Name' },
				isVirtual: false
			},
			{
				code: 'msVal2',
				name: { fi: 'msVal2Name' },
				isVirtual: false
			}
		]
	}
];

const mockVisualizationSettings: IVisualizationSettings = {
	multiselectableVariableCode: 'msVar'
};
const mockVisualizationRules: IVisualizationRules = {
	allowManualPivot: false, sortingOptions: null, multiselectDimensionAllowed: true
};

jest.mock('react-i18next', () => ({
	...jest.requireActual('react-i18next'),
	useTranslation: () => {
		return {
			t: (str: string) => str,
			i18n: {
				changeLanguage: () => new Promise(() => null),
			},
		};
	},
}));

describe('Rendering test', () => {
	it('renders correctly', () => {
		const { asFragment } = render(
			<MultiselectableSelector
				visualizationRules={mockVisualizationRules}
				visualizationSettings={mockVisualizationSettings}
				dimensions={mockDimensions}
			/>);
		expect(asFragment()).toMatchSnapshot();
	});
});



describe('Assertion tests', () => {
	it('When no multiselect variable code is provided, the selector should default to "noMultiselectable"', () => {
		render(<MultiselectableSelector
			dimensions={mockDimensions}
			visualizationRules={mockVisualizationRules}
			visualizationSettings={{ ...mockVisualizationSettings, multiselectableVariableCode: null }}
		></MultiselectableSelector>);
		expect(screen.getByDisplayValue("noMultiselectable")).toBeInTheDocument();
	});

	it('When multiselect variable code is provided, the selector should be rendered with a corresponding value', () => {
		const changedMockSettings = { ...mockVisualizationSettings, multiselectableDimensionCode: "msVar" }
		render(<MultiselectableSelector
			dimensions={mockDimensions}
			visualizationRules={mockVisualizationRules}
			visualizationSettings={changedMockSettings}
		></MultiselectableSelector>);
		expect(screen.queryByDisplayValue("msVar")).toBeInTheDocument();
	});
});