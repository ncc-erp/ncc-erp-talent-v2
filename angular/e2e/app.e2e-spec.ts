import { TalentV2TemplatePage } from './app.po';

describe('TalentV2 App', function () {
  let page: TalentV2TemplatePage;

  beforeEach(() => {
    page = new TalentV2TemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual("app works!");
  });
});
