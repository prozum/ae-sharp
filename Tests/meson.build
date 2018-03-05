nunit = dependency('nunit')

nunit_console = find_program('nunit-console')

aesharp_test_lib = library('aesharp', 'AESharp.Tests.cs', dependencies: nunit)
test('nunit test', nunit_console, args: aesharp_test_lib)
